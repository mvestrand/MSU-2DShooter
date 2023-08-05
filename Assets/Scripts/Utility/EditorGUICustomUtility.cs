using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class EditorGUICustomUtility {
    public const float minWeight = 0.001f;

    public static void ComputeRects(Rect position, Rect[] rects, IReadOnlyList<float> minWidths, IReadOnlyList<float> weights) {
        float totalMinWidth = minWidths.Sum();
        float totalWeight = weights.Sum();
        float extraWidth = position.width - totalMinWidth;

        float[] widths = new float[rects.Length];
        AllocateByWeight(position.width, ref widths, minWidths, weights);

        float usedWidth = 0;
        for (int i = 0; i < rects.Length; i++) {
            rects[i] = new Rect(position.x + usedWidth, position.y, Mathf.Min(widths[i], position.width - usedWidth), position.height);
            usedWidth = Mathf.Min(usedWidth + widths[i], position.width);
        }
    }

    public static void AllocateByWeight(float supply, ref float[] allocated, IReadOnlyList<float> desired, IReadOnlyList<float> weight) {
        float totalDesired = desired.Sum();
        float totalWeight = weight.Sum();

        for (int i = 0; i < allocated.Length; i++) { allocated[i] = 0; }
        if (supply <= float.Epsilon*10) {
            return;
        }

        if (supply >= totalDesired) { // Enough for everyone
            float surplus = supply - totalDesired;
            for (int i = 0; i < allocated.Length; i++) {
                allocated[i] = desired[i] + surplus * weight[i] / totalWeight;
            }
            return;
        } else { // Compete using the given weights
            float unclaimed = supply;
            while (unclaimed > float.Epsilon*10) {
                
                // Accumulate the total weight of the competitors
                totalWeight = 0;
                for (int i = 0; i < allocated.Length; i++) {
                    if (allocated[i] < desired[i]) {
                        totalWeight += Mathf.Max(weight[i], minWeight);
                    }
                }

                if (totalWeight == 0) // No competitors; Shouldn't ever happen
                    return;

                // Take from available supply using relative competitor weights
                for (int i = 0; i < allocated.Length; i++) {
                    if (allocated[i] < desired[i]) {
                        float claimable = unclaimed * Mathf.Max(weight[i], minWeight) / totalWeight;
                        if (claimable + allocated[i] >= desired[i]) { // Can take enough
                            unclaimed -= (desired[i] - allocated[i]);
                            allocated[i] = desired[i];
                        } else { // Wants more than it can take
                            allocated[i] += claimable;
                            unclaimed -= claimable;
                        }
                        if (unclaimed <= 0) // No more supply
                            return;
                    }
                }
            }
        }
    }}
