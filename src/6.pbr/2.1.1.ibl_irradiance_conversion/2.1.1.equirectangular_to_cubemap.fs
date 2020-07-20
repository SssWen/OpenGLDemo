#version 330 core
out vec4 FragColor;
in vec3 WorldPos;

uniform sampler2D equirectangularMap;// 这个贴图是柱状投影图,其实是从球面 投影到 圆柱面的 展开就是一个普通的2维背景图,而不是6个面的立方体图,本来也可以直接采样的,但是 \
// 6 个面的立方体贴图效率更高,所以这里给他重新转换成6个面的HDR图.

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
