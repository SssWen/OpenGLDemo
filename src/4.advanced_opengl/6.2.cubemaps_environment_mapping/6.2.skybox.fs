#version 330 core
out vec4 FragColor;

in vec3 TexCoords;

uniform samplerCube skybox;

void main()
{    
    FragColor = texture(skybox, TexCoords); // ����ֻ��Ҫ��local���귶Χ��-1,1������������в�����������
}