#ifndef FORCE_SHIELD_HITS
#define FORCE_SHIELD_HITS

int _HitsCount = 0;
float _HitsRadius[10];
float3 _HitPositions[10];
float _HitsIntensity[10];

float DrawRing(float intensity, float radius, float dist, float border)
{
    float currentRadius = lerp(0, radius, 1 - intensity);
    return intensity * (1 - smoothstep(currentRadius, currentRadius + border, dist) - (1 - smoothstep(currentRadius - border, currentRadius, dist)));
}

void CalculateHitsFactor_float(float3 objPosition, float border, out float factor) 
{
    factor = 0;
    for (int i = 0; i < _HitsCount; i++) {
        float distanceToHit = distance(objPosition, _HitPositions[i]);
        factor += DrawRing(_HitsIntensity[i], _HitsRadius[i], distanceToHit, border);
    }
    factor = saturate(factor);
}

#endif
