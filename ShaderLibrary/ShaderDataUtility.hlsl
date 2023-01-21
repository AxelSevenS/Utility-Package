#ifndef SHADER_DATA_UTILITIES_INCLUDED
#define SHADER_DATA_UTILITIES_INCLUDED


struct VertLightingInput {
    float4 clipPosition;
    float3 worldPosition;
    half3 worldNormal;
    half3 worldViewDirection;
    float4 screenPosition;
};

struct FragLightingInput {
    float4 clipPosition;
    float3 worldPosition;
    half3 worldNormal;
    half3 worldViewDirection;
    float4 screenPosition;
    float4 shadowCoord;
};

VertLightingInput GetVertLightingInput(half3 ObjectPosition, half3 ObjectNormal) {

    VertLightingInput lightingInput;
    lightingInput.clipPosition = TransformObjectToHClip(ObjectPosition);
    
    lightingInput.worldPosition = TransformObjectToWorld(ObjectPosition.xyz);
    lightingInput.worldNormal = normalize(TransformObjectToWorldNormal(ObjectNormal.xyz));
    lightingInput.worldViewDirection = normalize( _WorldSpaceCameraPos.xyz - lightingInput.worldPosition );
    #ifdef SHADERGRAPH_PREVIEW
        lightingInput.screenPosition = float4(0,0,0,0);
    #else
        lightingInput.screenPosition = ComputeScreenPos( lightingInput.clipPosition );
    #endif
    return lightingInput;
}

FragLightingInput GetFragLightingInput(VertLightingInput lightingInput) {
    FragLightingInput fragLightingInput;
    fragLightingInput.clipPosition = lightingInput.clipPosition;
    fragLightingInput.worldPosition = lightingInput.worldPosition;
    fragLightingInput.worldNormal = lightingInput.worldNormal;
    fragLightingInput.worldViewDirection = lightingInput.worldViewDirection;
    fragLightingInput.screenPosition = lightingInput.screenPosition;

    #ifdef SHADERGRAPH_PREVIEW
        fragLightingInput.shadowCoord = float4(0,0,0,0);
    #else
        #if SHADOWS_SCREEN
            fragLightingInput.shadowCoord = fragLightingInput.screenPosition;
        #else 
            fragLightingInput.shadowCoord = TransformWorldToShadowCoord(fragLightingInput.worldPosition);
        #endif
    #endif
    return fragLightingInput;
}


#endif