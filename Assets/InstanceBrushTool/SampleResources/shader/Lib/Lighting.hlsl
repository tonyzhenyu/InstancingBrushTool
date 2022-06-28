#ifndef LIGHTING_INCLUDED
#define LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

#define LIGHT_MAX_VALUE 1.0
#define LIGHT_MIN_VALUE 0.32

half3 GetViewDir() {
	return normalize(mul((float3x3)UNITY_MATRIX_I_V, half3(0, 0, 1)));
}

half4 GetMainLightColor() {
	return _MainLightColor;
}

float3 GetMainLightDirection() {
	return _MainLightPosition.xyz;
}

half GetMainLightDiffuse(float3 wnormal) {
	float3 mlight_direction = _MainLightPosition.xyz;
	return (dot(mlight_direction, wnormal) + 1) * 0.5;
}

half GetMainLightShadow(float3 wpos) {
	half4 shadowcoord = TransformWorldToShadowCoord(wpos);
	half shadow = MainLightRealtimeShadow(shadowcoord);
	return shadow;
}

half GetMainLightAttenuation(float3 wcampos, float3 wpos) {
	half frac = 1 / _MainLightColor.a * 0.017;
	return 1 / pow(2.71828, pow(distance(wcampos, wpos) * frac, 2));
}

half GetMainLightSpecular(float3 wnormal, float3 viewdir) {
	float3 light = _MainLightPosition.xyz;
	float3 reflection = 2 * dot(wnormal, light) * wnormal - light;
	return dot(reflection, viewdir);
}

half4 GetAmbientLightColor() {
	return _GlossyEnvironmentColor;
}

half4 CalculateLambertWithSpecular(float3 wnormal, float3 wpos, float3 wcampos, float gloss, float specular, float3 viewdir) {
	half diffuse = GetMainLightDiffuse(wnormal);
	half shadow = GetMainLightShadow(wpos);
	half attenuation = GetMainLightAttenuation(wcampos, wpos);
	half4 mlight_color = GetMainLightColor();

	half3 lambert = lerp(mlight_color.rgb, GetAmbientLightColor().rgb, 1 - diffuse);
	half spec = GetMainLightSpecular(wnormal, viewdir);
	spec *= specular * attenuation * clamp(shadow, 0.7, 1.0);
	spec = clamp(spec, 0, 1);
	spec = pow(spec, gloss);

	half lighting = clamp(diffuse * shadow * attenuation * mlight_color.a, LIGHT_MIN_VALUE, mlight_color.a);

	return half4((spec + lambert) * lighting, 1.0);
}


half4 CalculateLambert(float3 wnormal, float3 wpos, float3 wcampos) {
	half diffuse = GetMainLightDiffuse(wnormal);
	half shadow = GetMainLightShadow(wpos);
	half attenuation = GetMainLightAttenuation(wcampos, wpos);
	half4 mlight_color = GetMainLightColor();

	half3 lambert = lerp(mlight_color.rgb, GetAmbientLightColor().rgb, 1 - diffuse);

	half lighting = clamp(diffuse * shadow * attenuation * mlight_color.a, LIGHT_MIN_VALUE, mlight_color.a);

	return half4(lambert * lighting, 1.0);
}


#endif
