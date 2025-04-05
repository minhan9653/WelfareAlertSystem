using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Trainers;

namespace GyeotaeAdmin.ML
{
    public class RecommendationInput
    {
        public string userId { get; set; }
        public string itemId { get; set; }
        public float Label { get; set; }
    }

    public class RecommendationPrediction
    {
        public float Score { get; set; }
    }

    public static class MlRecommender
    {
        public static ITransformer TrainModel(MLContext mlContext, List<RecommendationInput> data)
        {
            var trainingData = mlContext.Data.LoadFromEnumerable(data);

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("userId")
                .Append(mlContext.Transforms.Conversion.MapValueToKey("itemId"))
                .Append(mlContext.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "userId",
                    MatrixRowIndexColumnName = "itemId",
                    LabelColumnName = "Label",
                    NumberOfIterations = 30,
                    ApproximationRank = 100
                }));

            return pipeline.Fit(trainingData); // ✅ 이제 오류 없음
        }

        public static List<(string itemId, float score)> RecommendForUser(
            MLContext mlContext,
            ITransformer model,
            List<RecommendationInput> allData,
            string targetUserId,
            int topN = 5)
        {
            var knownItems = allData
                .Where(x => x.userId == targetUserId && x.Label >= 1f)
                .Select(x => x.itemId)
                .ToHashSet();

            var candidateItems = allData
                .Select(x => x.itemId)
                .Distinct()
                .Where(item => !knownItems.Contains(item))
                .ToList();

            var predictionEngine = mlContext.Model.CreatePredictionEngine<RecommendationInput, RecommendationPrediction>(model);

            var predictions = candidateItems
                .Select(item => (
                    item,
                    score: predictionEngine.Predict(new RecommendationInput
                    {
                        userId = targetUserId,
                        itemId = item
                    }).Score))
                .OrderByDescending(x => x.score)
                .Take(topN)
                .ToList();

            return predictions;
        }

        public static List<RecommendationInput> ConvertToTrainingData(IEnumerable<object> users)
        {
            var list = new List<RecommendationInput>();

            foreach (var userObj in users)
            {
                var dict = (IDictionary<string, object>)userObj;
                var userId = dict["Phone"]?.ToString();

                foreach (var kv in dict)
                {
                    if (kv.Key == "Phone" || kv.Key == "Name") continue;

                    list.Add(new RecommendationInput
                    {
                        userId = userId,
                        itemId = kv.Key,
                        Label = Convert.ToSingle(kv.Value)
                    });
                }
            }

            return list;
        }

        public static List<(string itemId, float averageScore)> PredictGlobalProgramInterest(
            MLContext mlContext,
            ITransformer model,
            List<RecommendationInput> data,
            int topN = 5)
        {
            var allUserIds = data.Select(d => d.userId).Distinct();
            var allItemIds = data.Select(d => d.itemId).Distinct();

            var predictionEngine = mlContext.Model.CreatePredictionEngine<RecommendationInput, RecommendationPrediction>(model);

            var scores = new Dictionary<string, List<float>>();

            foreach (var userId in allUserIds)
            {
                foreach (var itemId in allItemIds)
                {
                    var prediction = predictionEngine.Predict(new RecommendationInput
                    {
                        userId = userId,
                        itemId = itemId
                    });

                    if (!scores.ContainsKey(itemId))
                        scores[itemId] = new List<float>();

                    scores[itemId].Add(prediction.Score);
                }
            }

            return scores
                .Select(kv => (itemId: kv.Key, averageScore: kv.Value.Average()))
                .OrderByDescending(x => x.averageScore)
                .Take(topN)
                .ToList();
        }
    }
}
