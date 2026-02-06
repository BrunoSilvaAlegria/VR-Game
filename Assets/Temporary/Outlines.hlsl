struct ScharrOperators {
    float3x3 x;
    float3x3 y;
};

ScharrOperators GetEdgeDetectionKernels() {
    ScharrOperators kernels;
    kernels.x = float3x3(
        -3, -10, -3,
         0,   0,  0,
         3,  10,  3
    );
    kernels.y = float3x3(
        -3,  0,  3,
       -10,  0, 10,
        -3,  0,  3
    );
    return kernels;
}

void DepthBasedOutlines_float(float2 screenUV, float2 px, out float outlines)
{
    outlines = 0;

    float gx = 0;
    float gy = 0;

    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            if (i == 0 && j == 0) continue;

            float2 offset = float2(i, j) * px;

            float d = SHADERGRAPH_SAMPLE_SCENE_DEPTH(screenUV + offset);

            float kx =
                (i == -1 && j == -1) ? -3 :
                (i == -1 && j ==  0) ? -10 :
                (i == -1 && j ==  1) ? -3 :
                (i ==  1 && j == -1) ?  3 :
                (i ==  1 && j ==  0) ? 10 :
                (i ==  1 && j ==  1) ?  3 : 0;

            float ky =
                (i == -1 && j == -1) ? -3 :
                (i ==  0 && j == -1) ? -10 :
                (i ==  1 && j == -1) ? -3 :
                (i == -1 && j ==  1) ?  3 :
                (i ==  0 && j ==  1) ? 10 :
                (i ==  1 && j ==  1) ?  3 : 0;

            gx += d * kx;
            gy += d * ky;
        }
    }

    float g = sqrt(gx * gx + gy * gy);
    outlines = step(0.02, g);
}

//void NormalBasedOutlines_float(float2 screenUV, float2 px, out float outlines)
