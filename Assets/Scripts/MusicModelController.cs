using UnityEngine;
using Unity.Barracuda;

public class MusicModelController : MonoBehaviour
{
    public NNModel musicModelAsset;
    private Model runtimeModel;
    private IWorker worker;

    // Input normalization parameters (from your training scaler for inputs)
    // Input Mean: [3.445, 15.247, 621.40217027]
    // Input Std:  [1.71200905, 8.92849321, 359.8968479]
    public Vector3 inputMean = new Vector3(3.445f, 15.247f, 621.40217027f);
    public Vector3 inputStd  = new Vector3(1.71200905f, 8.92849321f, 359.8968479f);

    // Output inverse transformation parameters (from your training scaler for outputs)
    // Output Mean: [0.498, 107.29541003, 0.84068]
    // Output Std:  [0.499996, 11.52709076, 0.11822833]
    public Vector3 outputMean = new Vector3(0.498f, 107.29541003f, 0.84068f);
    public Vector3 outputStd  = new Vector3(0.499996f, 11.52709076f, 0.11822833f);

    void Start()
    {
        runtimeModel = ModelLoader.Load(musicModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }

    // Evaluate the model using 3 input parameters: numPatients, totalInjuries, totalTimeLeft.
    // The method normalizes the inputs, runs the model, then applies the inverse transformation to the outputs.
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
