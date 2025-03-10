using UnityEngine;
using Unity.Barracuda;

public class MusicModelController : MonoBehaviour
{
    public NNModel musicModelAsset;
    private Model runtimeModel;
    private IWorker worker;

    // Input normalization parameters (from your training scaler for inputs)
    // Input Mean: [3.47, 15.296, 617.46335079]
    // Input Std:  [1.74272775, 8.83155615, 363.25787182]
    public Vector3 inputMean = new Vector3(3.47f, 15.296f, 617.46335079f);
    public Vector3 inputStd  = new Vector3(1.74272775f, 8.83155615f, 363.25787182f);

    // Output inverse transformation parameters (from your training scaler for outputs)
    // Output Mean: [0.512, 130.07831994, 0.84223]
    // Output Std:  [0.49985598, 20.42676928, 0.11949614]
    public Vector3 outputMean = new Vector3(0.512f, 130.07831994f, 0.84223f);
    public Vector3 outputStd  = new Vector3(0.49985598f, 20.42676928f, 0.11949614f);

    void Start()
    {
        runtimeModel = ModelLoader.Load(musicModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }

    // Evaluate the model using 3 input parameters: numPatients, totalInjuries, totalTimeLeft.
    // This method normalizes the raw inputs, executes the model,
    // and then applies the inverse transformation to the outputs.
    public float[] EvaluateModel(float numPatients, float totalInjuries, float totalTimeLeft)
    {
        // Normalize inputs:
        float normPatients = (numPatients - inputMean.x) / inputStd.x;
        float normInjuries = (totalInjuries - inputMean.y) / inputStd.y;
        float normTimeLeft = (totalTimeLeft - inputMean.z) / inputStd.z;

        // Create a 1x3 input tensor with normalized values.
        Tensor inputTensor = new Tensor(1, 3);
        inputTensor[0, 0] = normPatients;
        inputTensor[0, 1] = normInjuries;
        inputTensor[0, 2] = normTimeLeft;

        // Execute the model.
        worker.Execute(inputTensor);

        // Retrieve the raw outputs from the "output" layer.
        Tensor outputTensor = worker.PeekOutput("output");
        float[] modelOutput = new float[3];
        for (int i = 0; i < 3; i++)
        {
            modelOutput[i] = outputTensor[0, i];
        }

        // Apply inverse transformation to each output:
        // realValue = (normalizedValue * std) + mean
        modelOutput[0] = modelOutput[0] * outputStd.x + outputMean.x; // musicTrack
        modelOutput[1] = modelOutput[1] * outputStd.y + outputMean.y; // tempo
        modelOutput[2] = modelOutput[2] * outputStd.z + outputMean.z; // volume

        // Dispose tensors to free resources.
        inputTensor.Dispose();
        outputTensor.Dispose();

        return modelOutput;
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
