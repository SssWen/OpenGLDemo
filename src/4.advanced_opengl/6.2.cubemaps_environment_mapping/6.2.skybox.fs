#version 330 core
out vec4 FragColor;

in vec3 TexCoords;

uniform samplerCube skybox;

void main()
{    
    FragColor = texture(skybox, TexCoords); // 这里只需要用local坐标范围是-1,1的物体坐标进行采样球面坐标
}