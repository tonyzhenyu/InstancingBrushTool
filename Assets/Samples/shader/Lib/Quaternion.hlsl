#ifndef QUATERNION_INCLUDED
#define QUATERNION_INCLUDED

float4 quat_mul(float4 _qa, float4 _qb) {
	float3 va = _qa.xyz;
	float3 vb = _qb.xyz;
	float wa = _qa.w;
	float wb = _qb.w;

	float dotab = dot(va, vb);
	float3 crossab = cross(va, vb);
	return float4(wa * vb + wb * va + crossab, wa * wb - dotab);
}

float4 quat_axis_angle(float3 _axis, float _radian) {
	float t = sin(_radian * 0.5);
	float3 naxis = normalize(_axis);
	return float4(naxis.x * t, naxis.y * t, naxis.z * t, cos(_radian * 0.5));
}

float3 quat_apply(float3 v, float4 q) {
	float3 qv = float3(q.x, q.y, q.z);
	float3 t = 2 * cross(qv, v);
	return v + q.w * t + cross(qv, t);
}

float4 quat_vectovec(float3 va, float3 vb) {
	float4 quat = float4(cross(va, vb), 0);
	float va_sqlength = va.x * va.x + va.y * va.y + va.z * va.z;
	float vb_sqlength = vb.x * vb.x + vb.y * vb.y + vb.z * vb.z;
	quat.w = sqrt(va_sqlength * vb_sqlength + dot(va, vb));

	return normalize(quat);
}

// FIXME: to be corrected
float4 quat_lookat(float3 dir, float3 up) {
	float3 rup = cross(cross(dir, up), dir);
    float4 qa = quat_vectovec(float3(0, 0, 1), dir);
	float3 wup = quat_apply(up, qa);
	float4 qb = quat_vectovec(wup, rup);
	float4 quat = quat_mul(qb, qa);
    return quat;
}

#endif
