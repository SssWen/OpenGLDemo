#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D depthMap;
uniform float near_plane;
uniform float far_plane;

// required when using a perspective projection matrix
// �������Ե�depthֵ,ת��Ϊ���Ե�depthֵ,�����������Թ�Դλ����Ⱦ�Ŀռ��¼������ֵ,����ʹ�õĻ��ǹ�Դλ�õĽ�ƽ���Զƽ��,��ԭ���Ե�depth.
float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0; // Back to NDC 
    return (2.0 * near_plane * far_plane) / (far_plane + near_plane - z * (far_plane - near_plane));	
}

void main()
{             
    // ƽ�й�� shadowmap�����Ե�,��Ϊʹ�õ�������ͶӰ,
    // �����͸��ͶӰ�Ļ�,��Ҫ�����������ֵ,���»��㵽����.
    float depthValue = texture(depthMap, TexCoords).r;
    // FragColor = vec4(vec3(LinearizeDepth(depthValue) / far_plane), 1.0); // perspective
    FragColor = vec4(vec3(depthValue), 1.0); // orthographic
}