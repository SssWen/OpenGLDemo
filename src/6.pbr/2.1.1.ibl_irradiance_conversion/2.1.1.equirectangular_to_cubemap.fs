#version 330 core
out vec4 FragColor;
in vec3 WorldPos;

uniform sampler2D equirectangularMap;
// 柱面HRD图（长方形）-> 6个面立方体环境贴图
// 这个贴图是柱状投影图,其实是从球面 投影到 圆柱面的 展开就是一个普通的2维背景图,而不是6个面的立方体图,本来也可以直接采样的,但是 \
// 6 个面的立方体贴图效率更高,所以这里给他重新转换成6个面的HDR图.

// https://en.wikipedia.org/wiki/Spherical_coordinate_system
// https://www.scratchapixel.com/lessons/mathematics-physics-for-computer-graphics/mathematics-of-shading
// 用直角坐标系（xyz）表示球面坐标系（r,φ, θ）
// r = (x^2+y^2+z^2)2^(1/2)
// u: ϕ = tan−1(z/x) --- tanϕ = z/x ,与x轴的夹角,类似维度，0~2π
// v: θ = cos−1(y/r) --- cosθ = y/r ,与y轴的夹角,类似经度，0~π

// atan(v.z, v.x) 范围 -2π~2π
// asin(v.y) 范围 -π ~ π
// 1/π = 0.3183，  1/2π = 0.1591

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
    vec3 color = texture(equirectangularMap, uv).rgb; // 理解为根据每个面的角度，转换为UV，采样HDR矩形贴图，投影到6个面。
    
    FragColor = vec4(color, 1.0);
}
