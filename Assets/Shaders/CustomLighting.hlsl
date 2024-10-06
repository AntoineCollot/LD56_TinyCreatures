#ifdef SHADERGRAPH_PREVIEW
#else
half3 GetAdditionalLightColor(float3 Normal, float3 WPos, out half Intensity)
{
	Light addLight = GetAdditionalLight(0, WPos, 1);
	half lightDot = dot(Normal, addLight.direction);
	Intensity = smoothstep(0, 0.02, lightDot* addLight.distanceAttenuation) * addLight.shadowAttenuation * saturate(GetAdditionalLightsCount());

	return lerp(0,addLight.color,Intensity);
}
#endif

void LibraryShading_float(in float3 Normal, in float3 ClipSpacePos, in float3 WorldPos, in float3 ViewDir,in float Glossiness, out float3 Output)
{

half3 _ShadowColor = half3(0,0,0);
half3 _GlossinessColor = half3(1,1,1);

	// set the shader graph node previews
#ifdef SHADERGRAPH_PREVIEW
	Output = float3(0.5, 0.5, 0);
#else

	// grab the shadow coordinates
#if SHADOWS_SCREEN
	half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
#else
	half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif 

	// grab the main light
#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
	Light light = GetMainLight(shadowCoord);
#else
	Light light = GetMainLight();
#endif

//Additional Lights
	half addLightIntensity = 0;
	float3 addLightCol = float3(0,0,0);
#ifdef _ADDITIONAL_LIGHTS
	addLightCol = GetAdditionalLightColor(Normal, WorldPos, addLightIntensity);
#endif

	//Rework glossiness to better handle small values, add a fixed min glossiness
	Glossiness = saturate(Glossiness * Glossiness *Glossiness  + 0.2);
	
	//Light
	half NdotL = dot(Normal, light.direction);
	half lightIntensity =NdotL * light.shadowAttenuation;
	half3 lightTint = lerp(_ShadowColor, light.color, saturate(lightIntensity));

	//Specular
	//Half vector is the vetwor in between view dir and light dir
	float3 halfVector = normalize(ViewDir+light.direction);
	float NdotH = dot(Normal, halfVector);
	//Change size based on glossiness
	float specularIntensity = pow(NdotH * lightIntensity, Glossiness *400);
	//Change intensity based on glossiness
	specularIntensity = max(smoothstep(0.005, 0.03, specularIntensity), addLightIntensity) * Glossiness;

	Output = lightTint + specularIntensity *_GlossinessColor;
	
	Output = lerp(_ShadowColor, Output, addLightCol);
	
	//	Light addLight = GetAdditionalLight(0, WorldPos, 1);
	//Output = addLight.shadowAttenuation;
#endif
}