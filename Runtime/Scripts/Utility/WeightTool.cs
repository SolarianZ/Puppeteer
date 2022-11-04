using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Utility
{
    public static class WeightTool
    {
        public static void NormalizeWeights(IList<float> originalWeights, IList<float> normalizedWeights)
        {
            Assert.AreEqual(originalWeights.Count, normalizedWeights.Count,
                "Original weight count not match with normalized weight count.");

            // Normalize input weights
            var totalWeight = 0f;
            for (int i = 0; i < originalWeights.Count; i++)
            {
                var currWeight = originalWeights[i];
                if (currWeight < 0)
                {
                    Debug.LogError($"Force increase the original weight '{currWeight}' of input '{i}' to 0.");
                    currWeight = 0;
                }

                totalWeight += currWeight;
            }

            if (totalWeight == 0)
            {
                for (int i = 0; i < originalWeights.Count; i++)
                {
                    normalizedWeights[i] = 0;
                }
            }
            else
            {
                for (int i = 0; i < originalWeights.Count; i++)
                {
                    normalizedWeights[i] = originalWeights[i] / totalWeight;
                }
            }
        }
    }
}
