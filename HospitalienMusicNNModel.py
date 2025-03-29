import pandas as pd
import numpy as np
import torch
import torch.nn as nn
from torch.utils.data import Dataset, DataLoader, Subset
from sklearn.model_selection import KFold
from sklearn.preprocessing import StandardScaler
from sklearn.metrics import mean_squared_error
import matplotlib.pyplot as plt
import os

# 1) CUSTOM DATASET
class MusicDataset(Dataset):
    def __init__(self, csv_file):
        self.data = pd.read_csv(csv_file)
        """
        CSV columns order:
        [PatientCount, InjuryCount, TimeLeft, ActiveTrack, Tempo, 
         Song1_MelodyVol, Song1_DrumsVol, Song2_MelodyVol, Song2_DrumsVol, Song2_BassVol,
         Song3_HiPassVol, Song3_LowPassVol]
        Inputs (X): first 3 columns
        Outputs (y): next 9 columns
        """
        self.X = self.data.iloc[:, 0:3].values.astype(np.float32)
        self.y = self.data.iloc[:, 3:12].values.astype(np.float32)
        
        self.scaler_X = StandardScaler()
        self.scaler_y = StandardScaler()
        self.X = self.scaler_X.fit_transform(self.X)
        self.y = self.scaler_y.fit_transform(self.y)
        
        print("Input Mean:", self.scaler_X.mean_)
        print("Input Std:", self.scaler_X.scale_)
        print("Output Mean:", self.scaler_y.mean_)
        print("Output Std:", self.scaler_y.scale_)
        
    def __len__(self):
        return len(self.data)
    
    def __getitem__(self, idx):
        return self.X[idx], self.y[idx]
    
    def get_scalers(self):
        return self.scaler_X, self.scaler_y

# 2) HELPER FUNCTION TO BUILD MODEL WITH VARIABLE ARCHITECTURE AND DROPOUT
def build_model(input_size, hidden_size, output_size, num_layers, dropout_rate):
    layers = []
    layers.append(nn.Linear(input_size, hidden_size))
    layers.append(nn.ReLU())
    if dropout_rate > 0:
        layers.append(nn.Dropout(dropout_rate))
    # Create (num_layers - 1) additional hidden layers
    for _ in range(num_layers - 1):
        layers.append(nn.Linear(hidden_size, hidden_size))
        layers.append(nn.ReLU())
        if dropout_rate > 0:
            layers.append(nn.Dropout(dropout_rate))
    layers.append(nn.Linear(hidden_size, output_size))
    return nn.Sequential(*layers)

def train_one_epoch(dataloader, model, loss_fn, optimizer):
    model.train()
    total_loss = 0.0
    for X, y in dataloader:
        pred = model(X)
        loss = loss_fn(pred, y)
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
        total_loss += loss.item()
    return total_loss / len(dataloader)

def validate(dataloader, model, loss_fn):
    model.eval()
    total_loss = 0.0
    with torch.no_grad():
        for X, y in dataloader:
            pred = model(X)
            loss = loss_fn(pred, y)
            total_loss += loss.item()
    return total_loss / len(dataloader)

def main_optimal_experiment():
    # Optimal parameters
    hidden_size = 128
    num_layers = 2              # 2 hidden layers
    dropout_rate = 0.3
    weight_decay = 0.001
    optimizer_choice = 'Adam'
    learning_rate = 0.002
    num_epochs = 400
    batch_size = 32
    patience = 50
    k_folds = 10
    
    # Load dataset using absolute path.
    dataset = MusicDataset(r"C:\Users\blobf.DESKTOP-IUEL8R6\AppData\LocalLow\Ubifox\HospitAlien\music_dataset.csv")
    scaler_X, scaler_y = dataset.get_scalers()
    indices = np.arange(len(dataset))
    
    fold_mses = []
    kf = KFold(n_splits=k_folds, shuffle=True, random_state=42)
    
    for fold, (train_idx, val_idx) in enumerate(kf.split(indices)):
        print(f"Starting fold {fold+1}/{k_folds}...")
        train_subset = Subset(dataset, train_idx)
        val_subset = Subset(dataset, val_idx)
        
        train_loader = DataLoader(train_subset, batch_size=batch_size, shuffle=True)
        val_loader = DataLoader(val_subset, batch_size=batch_size, shuffle=False)
        
        model = build_model(input_size=3, hidden_size=hidden_size, output_size=9,
                            num_layers=num_layers, dropout_rate=dropout_rate)
        loss_fn = nn.MSELoss()
        
        # Use Adam with the specified learning rate.
        optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate, weight_decay=weight_decay)
        
        best_val_loss = float("inf")
        epochs_no_improve = 0
        
        for epoch in range(num_epochs):
            train_loss = train_one_epoch(train_loader, model, loss_fn, optimizer)
            val_loss = validate(val_loader, model, loss_fn)
            if val_loss < best_val_loss:
                best_val_loss = val_loss
                epochs_no_improve = 0
            else:
                epochs_no_improve += 1
                if epochs_no_improve >= patience:
                    print(f"  Fold {fold+1}: Early stopping at epoch {epoch+1}")
                    break
            if (epoch+1) % 10 == 0:
                print(f"  Fold {fold+1}: Epoch {epoch+1} - Train Loss: {train_loss:.4f}, Val Loss: {val_loss:.4f}")
        
        model.eval()
        predictions = []
        targets = []
        with torch.no_grad():
            for X, y in val_loader:
                pred = model(X)
                predictions.append(pred.numpy())
                targets.append(y.numpy())
        predictions = np.concatenate(predictions, axis=0)
        targets = np.concatenate(targets, axis=0)
        predictions_orig = scaler_y.inverse_transform(predictions)
        targets_orig = scaler_y.inverse_transform(targets)
        fold_mse = mean_squared_error(targets_orig, predictions_orig)
        fold_mses.append(fold_mse)
        print(f"  Fold {fold+1} completed: MSE = {fold_mse:.4f}")
    
    avg_mse = np.mean(fold_mses)
    print("\nFinal Experiment Result:")
    print(f"Hidden Size: {hidden_size}, Hidden Layers: {num_layers}, Optimiser: {optimizer_choice}")
    print(f"Learning Rate: {learning_rate}, Epochs: {num_epochs}, Dropout: {dropout_rate}, Weight Decay: {weight_decay}")
    print(f"Average Validation MSE (CV): {avg_mse:.4f}")
    
    # ----- Export final model to ONNX -----
    # Train a final model on the entire dataset using the optimal parameters.
    print("\nTraining final model on the entire dataset for ONNX export...")
    full_loader = DataLoader(dataset, batch_size=batch_size, shuffle=True)
    final_model = build_model(input_size=3, hidden_size=hidden_size, output_size=9,
                              num_layers=num_layers, dropout_rate=dropout_rate)
    final_optimizer = torch.optim.Adam(final_model.parameters(), lr=learning_rate, weight_decay=weight_decay)
    # Reuse the same loss function.
    loss_fn = nn.MSELoss()
    for epoch in range(num_epochs):
        train_loss = train_one_epoch(full_loader, final_model, loss_fn, final_optimizer)
        if (epoch+1) % 50 == 0:
            print(f"Final Model - Epoch {epoch+1}: Train Loss = {train_loss:.4f}")
    
    # Create a dummy input with the correct input shape (batch size 1, 3 features).
    dummy_input = torch.randn(1, 3)
    onnx_path = os.path.join(os.getcwd(), "optimal_music_model.onnx")
    torch.onnx.export(final_model, dummy_input, onnx_path, input_names=["input"], output_names=["output"], verbose=True)
    print(f"Final model exported to ONNX at {onnx_path}")
    
if __name__ == "__main__":
    main_optimal_experiment()
