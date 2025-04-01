using UnityEngine;
using Unity.Barracuda;

public class MusicModelController : MonoBehaviour
{
    public NNModel musicModelAsset;
    private Model runtimeModel;
    private IWorker worker;

    // Updated input normalization parameters (from the new training scaler)
    // Input Mean: [3.10545024, 14.62440758, 533.78145158]
    // Input Std:  [1.96281031, 8.90936041, 310.85870281]
    private Vector3 inputMean = new Vector3(3.10545024f, 14.62440758f, 533.78145158f);
    private Vector3 inputStd  = new Vector3(1.96281031f, 8.90936041f, 310.85870281f);

    // Updated output inverse transformation parameters (from the new training scaler)
    // Output Mean: [0.97630332, 125.55557353, 0.21112221, 0.20068607, 0.23485669,
    //               0.22881629, 0.23403013, 0.25823629, 0.25833559]
    // Output Std:  [0.8228995, 17.07588504, 0.28940052, 0.27646851, 0.34807077,
    //               0.34028911, 0.34699155, 0.37669148, 0.37678594]
    private float[] outputMean = new float[] { 0.97630332f, 125.55557353f, 0.21112221f, 0.20068607f, 0.23485669f, 0.22881629f, 0.23403013f, 0.25823629f, 0.25833559f };
    private float[] outputStd  = new float[] { 0.8228995f, 17.07588504f, 0.28940052f, 0.27646851f, 0.34807077f, 0.34028911f, 0.34699155f, 0.37669148f, 0.37678594f };

    void Start()
    {
        runtimeModel = ModelLoader.Load(musicModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }

    // Evaluate the model using 3 input parameters: PatientCount, InjuryCount, TimeLeft.
    // The method normalizes the inputs, runs the model, then applies the inverse transformation to the 9 outputs.
    public float[] EvaluateModel(float patientCount, float injuryCount, float timeLeft)
    {
        // Normalize inputs
        float normPatient = (patientCount - inputMean.x) / inputStd.x;
        float normInjury = (injuryCount - inputMean.y) / inputStd.y;
        float normTime   = (timeLeft - inputMean.z) / inputStd.z;

        // Create a 1x3 input tensor with normalized values.
        Tensor inputTensor = new Tensor(1, 3);
        inputTensor[0, 0] = normPatient;
        inputTensor[0, 1] = normInjury;
        inputTensor[0, 2] = normTime;

        // Execute the model.
        worker.Execute(inputTensor);

        // Retrieve the raw outputs from the "output" layer.
        // Now the model outputs 9 values.
        Tensor outputTensor = worker.PeekOutput("output");
        float[] modelOutput = new float[9];
        for (int i = 0; i < 9; i++)
        {
            modelOutput[i] = outputTensor[0, i];
        }

        // Apply inverse transformation to each output:
        // realValue = (normalizedValue * std) + mean
        for (int i = 0; i < 9; i++)
        {
            modelOutput[i] = modelOutput[i] * outputStd[i] + outputMean[i];
        }

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
