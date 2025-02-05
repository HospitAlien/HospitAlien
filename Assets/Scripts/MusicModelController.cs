using UnityEngine;
using Unity.Barracuda;

public class MusicModelController : MonoBehaviour
{
    // Drag your ONNX model (NNModel asset) here in the Inspector
    public NNModel musicModelAsset;

    private Model runtimeModel;
    private IWorker worker;

    // Initialize the model
    void Start()
    {
        runtimeModel = ModelLoader.Load(musicModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }

    // Evaluate the model using your 4 input parameters
    public float[] EvaluateModel(float avgInjurySeverity, float numPatients, float avgKnowledge, float sumTimeLeft)
    {
        // 1) Create a 1x4 input tensor
        Tensor inputTensor = new Tensor(1, 4);
        inputTensor[0, 0] = avgInjurySeverity;
        inputTensor[0, 1] = numPatients;
        inputTensor[0, 2] = avgKnowledge;
        inputTensor[0, 3] = sumTimeLeft;

        // 2) Execute the model
        worker.Execute(inputTensor);

        // 3) Get the output from the "output" layer (as defined in our PyTorch export script)
        Tensor outputTensor = worker.PeekOutput("output");

        // The model outputs 7 values: tempo, volume, percussionLevel, synthsLevel, intensity, useStrings, useHighPercussion
        float[] modelOutput = new float[7];
        for (int i = 0; i < 7; i++)
        {
            modelOutput[i] = outputTensor[0, i];
        }

        // Dispose to free resources
        inputTensor.Dispose();
        outputTensor.Dispose();

        return modelOutput; 
    }

    void OnDestroy()
    {
        // Clean up Barracuda worker
        worker?.Dispose();
    }
}
