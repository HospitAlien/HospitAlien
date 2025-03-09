using UnityEngine;
using Unity.Barracuda;

public class MusicModelController : MonoBehaviour
{
    public NNModel musicModelAsset;

    private Model runtimeModel;
    private IWorker worker;

    // Initialise the model
    void Start()
    {
        runtimeModel = ModelLoader.Load(musicModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }

    // Evaluate the model using the 3 input parameters: numPatients, totalInjuries, totalTimeLeft
    public float[] EvaluateModel(float numPatients, float totalInjuries, float totalTimeLeft)
    {
        // Create a 1x3 input tensor
        Tensor inputTensor = new Tensor(1, 3);
        inputTensor[0, 0] = numPatients;
        inputTensor[0, 1] = totalInjuries;
        inputTensor[0, 2] = totalTimeLeft;

        // Execute the model
        worker.Execute(inputTensor);

        // Get the output from the "output" layer (our exported model's output name)
        Tensor outputTensor = worker.PeekOutput("output");

        // The model outputs 3 values: musicTrack, tempo, volume
        float[] modelOutput = new float[3];
        for (int i = 0; i < 3; i++)
        {
            modelOutput[i] = outputTensor[0, i];
        }

        // Dispose tensors to free resources
        inputTensor.Dispose();
        outputTensor.Dispose();

        return modelOutput;
    }

    void OnDestroy()
    {
        // Clean up the Barracuda worker when the object is destroyed
        worker?.Dispose();
    }
}
