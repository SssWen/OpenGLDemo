#version 330 core
out vec4 FragColor;
in vec3 WorldPos;

uniform samplerCube environmentMap;

const float PI = 3.14159265359;

void main()
{		
	// The world vector acts as the normal of a tangent surface
    // from the origin, aligned to WorldPos. Given this normal, calculate all
    // incoming radiance of the environment. The result of this radiance
    // is the radiance of light coming from -Normal direction, which is what
    // we use in the PBR shader to sample irradiance.
    vec3 N = normalize(WorldPos); // 球的局部坐标，可以模拟球的法线方向

    vec3 irradiance = vec3(0.0);   
    
    // tangent space calculation from origin point
    vec3 up    = vec3(0.0, 1.0, 0.0);
    vec3 right = cross(up, N);
    up            = cross(N, right);
       
    float sampleDelta = 0.025;
    float nrSamples = 0.0;
    for(float phi = 0.0; phi < 2.0 * PI; phi += sampleDelta) // ϕ
	// 因为要把立方体的每个像素当作光源，所以可以对他进行卷积，生成辐照度图。
    //而生成漫反射的辐照度图,用来当作漫反射的光源,从环境立方体贴图中进行卷积,其实就是求平均值，积分操作,看起来像模糊操作。类似模糊算子。
    //对有限数量的方向采样以近似求解，在半球内均匀间隔或随机取方向可以获得一个相当精确的辐照度近似值，从而离散地计算积分 ∫ 。
    {
		// https://www.scratchapixel.com/lessons/mathematics-physics-for-computer-graphics/mathematics-of-shading
		// 计算球面坐标系 到 直角坐标系的转换		
		// x = rsin(θ)cos(ϕ)
		// y = rcos(θ)
		// z = rsin(θ)sin(ϕ)
        for(float theta = 0.0; theta < 0.5 * PI; theta += sampleDelta) // θ
        {
            // spherical to cartesian (in tangent space)
			// 切线空间 法线
            vec3 tangentSample = vec3(sin(theta) * cos(phi),  sin(theta) * sin(phi), cos(theta));
            // tangent space to world
			// 法线转换到世界空间，乘上法线矩阵，right(x,y,z),up(x,y,z),N(x,y,z) 组装成TBN矩阵，这里的TBN矩阵还有点问题		
            vec3 sampleVec = tangentSample.x * right + tangentSample.y * up + tangentSample.z * N; 
			// 得到了直角坐标系下坐标， 去采样 CubeMap得到辐照度贴图。
            irradiance += texture(environmentMap, sampleVec).rgb * cos(theta) * sin(theta);
            nrSamples++;
        }
    }
    irradiance = PI * irradiance * (1.0 / float(nrSamples));
    
    FragColor = vec4(irradiance, 1.0);
}
