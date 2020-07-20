#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D depthMap;
uniform float near_plane;
uniform float far_plane;

// required when using a perspective projection matrix
// 将非线性的depth值,转换为线性的depth值,还是重新在以光源位置渲染的空间下计算深度值,所以使用的还是光源位置的近平面和远平面,还原线性的depth.
float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0; // Back to NDC 
    return (2.0 * near_plane * far_plane) / (far_plane + near_plane - z * (far_plane - near_plane));	
}

void main()
{             
    // 平行光的 shadowmap是线性的,因为使用的是正交投影,
    // 如果是透视投影的话,需要将采样的深度值,重新换算到线性.
    float depthValue = texture(depthMap, TexCoords).r;
    // FragColor = vec4(vec3(LinearizeDepth(depthValue) / far_plane), 1.0); // perspective
    FragColor = vec4(vec3(depthValue), 1.0); // orthographic
}