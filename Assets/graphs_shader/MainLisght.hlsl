#ifndef  MAIN_LIGHT_INCLUDED
#define MAIN_LIGHT_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
struct EdgeConstants
{
    float diffuse;
     float shadowAttenuation;
     float distanceAttenuation;
     float specularOffset;
     float specular;
     float rimOffset;
     float rim;
    };
struct Variablessurface
{
  
float3 normal;
float3 view;
float smoothness;
float rimtreshhold;
float3 shinines;

EdgeConstants ec;
};

float3 CalculateCelShading(Light l,Variablessurface s)
{
    float shadowAttenuationSmoothStepped= smoothstep(0.0f,s.ec.shadowAttenuation,l.shadowAttenuation);
    float distanceAttenuationSmoothStepped= smoothstep(0.0f,s.ec.distanceAttenuation,l.distanceAttenuation);
  //float attenuation= l.shadowAttenuation*l.distanceAttenuation;
  float attenuation =shadowAttenuationSmoothStepped*distanceAttenuationSmoothStepped;
    float diffuse = saturate(dot(s.normal,l.direction));
    float3 h= SafeNormalize(l.direction+s.view);
    diffuse*=attenuation;
 //  diffuse = diffuse > 0 ? 1: 0;
    float specular = saturate(dot(s.normal,h)); 
    specular  =pow(specular,s.shinines);
    specular *= diffuse * s.smoothness;
     //specular = specular > 0.2 ? 1: 0;
    float rim =1 -dot(s.view, s.normal);
    rim *= pow(diffuse,s.rimtreshhold);
   //rim =rim >0.75 ? 1 :0;
   diffuse = smoothstep(0.0f, s.ec.diffuse,diffuse);
   specular= s.smoothness * smoothstep((1-s.smoothness)* s.ec.specular+s.ec.specularOffset, s.ec.specular+s.ec.specularOffset,specular);
   rim= s.smoothness * smoothstep(s.ec.rim-0.5f *s.ec.rimOffset,s.ec.rim+0.5f *s.ec.rimOffset, rim);
   return l.color*(diffuse+ max(specular,rim));
 // return attenuation;

 }
#endif

void MainLight_float (float Smoothness,float Rimtreshhold,float3 Position,float3 Normal,float3 View
    ,float EdgeDiffuse,float EdgeSpecular,float EdgeSpecularOffset,float EdgeDistanceAttenuation
    ,float EdgeShadowAttenuation,float EdgeRim,float EdgeRimOffset,out float3 Color)
{

//#ifdef SHADERGRAPH_PREVIEW
 #if defined( SHADERGRAPH_PREVIEW)

 Color = (0,1,1);

#else
 Variablessurface s;
 s.normal =normalize(Normal);
 s.view= SafeNormalize(View);
 s.smoothness=Smoothness;
 s.shinines= exp2(10*Smoothness+1);
 s.rimtreshhold=Rimtreshhold;


      s.ec.shadowAttenuation = EdgeShadowAttenuation;
        s.ec.distanceAttenuation = EdgeDistanceAttenuation;
        s.ec.diffuse=EdgeDiffuse;
        s.ec.specularOffset=EdgeSpecularOffset;
        s.ec.specular=EdgeSpecular;
        s.ec.rim=EdgeRim;
        s.ec.rimOffset=EdgeRimOffset;
 #if SHADOWS_SCREEN
 float4 clipPos = TransformWorldToHClip(Position);
 float4 shadowCoord = ComputeScreenPos(clipPos);
 #else
 float4 shadowCoord =TransformWorldToShadowCoord(Position);
 #endif
Light light =GetMainLight(shadowCoord);

Color = CalculateCelShading(light,s);

int pixelLightCount = GetAdditionalLightsCount();
for(int i= 0; i< pixelLightCount;i++)
{
    light= GetAdditionalLight(i,Position,1);
    Color+=CalculateCelShading(light,s);
    }
        
#endif

}
#endif