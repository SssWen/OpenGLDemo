#version 330 core
out vec4 FragColor;
in vec3 WorldPos;

uniform sampler2D equirectangularMap;// �����ͼ����״ͶӰͼ,��ʵ�Ǵ����� ͶӰ�� Բ����� չ������һ����ͨ��2ά����ͼ,������6�����������ͼ,����Ҳ����ֱ�Ӳ�����,���� \
// 6 �������������ͼЧ�ʸ���,���������������ת����6�����HDRͼ.

const vec2 invAtan = vec2(0.1591, 0.3183);
vec2 SampleSphericalMap(vec3 v)
{
    vec2 uv = vec2(atan(v.z, v.x), asin(v.y));
    uv *= invAtan;
    uv += 0.5;
    return uv;
}

void main()
{		
    vec2 uv = SampleSphericalMap(normalize(WorldPos));
    vec3 color = texture(equirectangularMap, uv).rgb;
    
    FragColor = vec4(color, 1.0);
}
