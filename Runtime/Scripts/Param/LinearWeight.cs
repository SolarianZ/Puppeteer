using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    public struct LinearWeight
    {
        public float OriginalWeight { get; private set; }

        public float RelativeWeight { get; private set; }

        public bool IsResolved { get; private set; }


        public LinearWeight(float originalWeight)
        {
            OriginalWeight = originalWeight;
            RelativeWeight = 0;
            IsResolved = false;
        }

        public void SetOriginalWeight(float originalWeight)
        {
            OriginalWeight = originalWeight;
            IsResolved = false;
        }

        public void Resolve(float totalWeight)
        {
            Assert.IsTrue(OriginalWeight >= 0 && OriginalWeight <= 1);
            Assert.IsTrue(totalWeight >= OriginalWeight);

            // totalWeight may changed externally, so here
            // should not be broke by internal resolve state
            // if (IsResolved) return;

            if (Mathf.Approximately(0, totalWeight))
            {
                RelativeWeight = 0;
            }
            else
            {
                RelativeWeight = OriginalWeight / totalWeight;
            }

            IsResolved = true;
        }
    }
}