#ifndef SOBEL_EDGE_DETECTION_INCLUDED
#define SOBEL_EDGE_DETECTION_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

TEXTURE2D(_DepthNormalsTexture);
SAMPLER(sampler_DepthNormalsTexture);

struct DepthNormalMap
{
    float3 Normal;
    float Depth;
};

DepthNormalMap SampleDepthNormalMap(in const float2 UV)
{
    const float4 CodedDepthAndNormals = SAMPLE_TEXTURE2D(_DepthNormalsTexture, sampler_DepthNormalsTexture, UV);

    DepthNormalMap Output;

    Output.Depth = dot(CodedDepthAndNormals.zw, float2(1.0, 1 / 255.0));

    const float Scale = 1.7777;
    const float3 Nn = CodedDepthAndNormals.xyz * float3(2 * Scale, 2 * Scale, 0) + float3(-Scale, -Scale, 1);
    const float G = 2.0 / dot(Nn.xyz, Nn.xyz);
    const float3 Normal01 = float3(G * Nn.xy, G - 1);
    Output.Normal = Normal01 * 2 - 1;

    return Output;
}

static const float2 GSobelSamplePoints[9] =
{
    float2(-1, +1), float2(+0, +1), float2(+1, +1),
    float2(-1, +0), float2(+0, +0), float2(+1, +0),
    float2(-1, -1), float2(+0, -1), float2(+1, -1),
};

static const float GSobelXMatrix[9] =
{
    +1, +0, -1,
    +2, +0, -2,
    +1, +0, -1,
};

static const float GSobelYMatrix[9] =
{
    +1, +2, +1,
    +0, +0, +0,
    -1, -2, -1,
};

float3 ScreenUVToViewDirection(in const float2 Input)
{
    const float2 POneOneTwoTwo = float2(unity_CameraProjection._11, unity_CameraProjection._22);
    return -normalize(float3((Input * 2 - 1) / POneOneTwoTwo, -1));
}

float ConvertSobelToEdgeMask(in const float Sobel, in const float Threshold, in const float Tightening, in const float Strength)
{
    return pow(smoothstep(0, Threshold, Sobel), Tightening) * Strength;
}

struct DepthAndNormalSobel
{
    float NormalSobel;
    float DepthSobel;
};

DepthAndNormalSobel CalculateDepthAndNormalSobel(in const float2 UV, in const float Thickness)
{
    float2 SobelD = 0, SobelX = 0, SobelY = 0, SobelZ = 0;

    [unroll] for (int Iterator = 0; Iterator < 9; ++Iterator)
    {
        // scene data
        const float Depth = SampleSceneDepth(UV + GSobelSamplePoints[Iterator] * Thickness);
        const float3 Normal = SampleDepthNormalMap(UV + GSobelSamplePoints[Iterator] * Thickness).Normal;

        // calculation data
        const float2 Kernel = float2(GSobelXMatrix[Iterator], GSobelYMatrix[Iterator]);

        SobelD += Depth * Kernel;
        SobelX += Normal.x * Kernel;
        SobelY += Normal.y * Kernel;
        SobelZ += Normal.z * Kernel;
    }

    DepthAndNormalSobel Output;
    Output.DepthSobel = length(SobelD);
    Output.NormalSobel = max(max(length(SobelX), length(SobelY)), length(SobelZ));
    return Output;
}

#endif