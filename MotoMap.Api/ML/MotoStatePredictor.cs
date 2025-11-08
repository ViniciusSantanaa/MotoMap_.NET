using Microsoft.ML;
using Microsoft.ML.Data;

namespace MotoMap.Api.ML;

public class MotoStatePredictor
{
    private readonly string _modelPath = Path.Combine(AppContext.BaseDirectory, "moto_model.zip");
    private readonly MLContext _mlContext = new MLContext();
    private PredictionEngine<ModelInput, ModelOutput>? _predEngine;

    public MotoStatePredictor()
    {
        if (File.Exists(_modelPath))
        {
            var loaded = _mlContext.Model.Load(_modelPath, out var schema);
            _predEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(loaded);
        }
        else
        {
            TrainAndSave();
        }
    }

    private void TrainAndSave()
    {
        var rnd = new Random();
        var labeled = new List<TempSample>();
        for (int i = 0; i < 1000; i++)
        {
            var rssi = (float)(rnd.NextDouble() * -80 + -20); // -100..-20
            var seconds = (float)rnd.Next(0, 3600);
            var label = (rssi > -60 && seconds < 300);
            labeled.Add(new TempSample { Rssi = rssi, SecondsSinceLastSeen = seconds, Label = label });
        }

        var data = _mlContext.Data.LoadFromEnumerable(labeled);
        var pipeline = _mlContext.Transforms.Concatenate("Features", nameof(TempSample.Rssi), nameof(TempSample.SecondsSinceLastSeen))
            .Append(_mlContext.BinaryClassification.Trainers.FastTree());

        var model = pipeline.Fit(data);
        _mlContext.Model.Save(model, data.Schema, _modelPath);
        _predEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
    }

    public ModelOutput Predict(ModelInput input)
    {
        if (_predEngine == null) throw new InvalidOperationException("Predictor not initialized.");
        return _predEngine.Predict(input);
    }

    private class TempSample
    {
        public float Rssi { get; set; }
        public float SecondsSinceLastSeen { get; set; }
        public bool Label { get; set; }
    }
}
