#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;

uniform vec3 samples[64];

// parameters (you'd probably want to use them as uniforms to more easily tweak the effect)
int kernelSize = 64;
float radius = 0.5;
float bias = 0.025;

// tile noise texture over screen based on screen dimensions divided by noise size
const vec2 noiseScale = vec2(1280.0/4.0, 720.0/4.0); 

uniform mat4 projection;

void main()
{
    // get input for SSAO algorithm
    vec3 fragPos = texture(gPosition, TexCoords).xyz;
    vec3 normal = normalize(texture(gNormal, TexCoords).rgb);
    vec3 randomVec = normalize(texture(texNoise, TexCoords * noiseScale).xyz);
    // create TBN change-of-basis matrix: from tangent-space to view-space
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);
    // iterate over the sample kernel and calculate occlusion factor
    float occlusion = 0.0;
    
    for(int i = 0; i < kernelSize; ++i)
    {
        // get sample position
		// 切线空间变换到相机空间
        vec3 sample = TBN * samples[i]; // from tangent to view-space
        sample = fragPos + sample * radius; 
        
        // project sample position (to sample texture) (to get position on screen/texture)
        vec4 offset = vec4(sample, 1.0);
        offset = projection * offset; // from view to clip-space
        offset.xyz /= offset.w; // perspective divide
        offset.xyz = offset.xyz * 0.5 + 0.5; // transform to range 0.0 - 1.0
        
        // get sample depth
        float sampleDepth = texture(gPosition, offset.xy).z; // get depth value of kernel sample
        
        // range check & accumulate
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(fragPos.z - sampleDepth));		
		// 如果采样的位置的深度值 大于 片段深度值【可以理解为在几何体里面】，该位置贡献 遮蔽因子
		// if the sample position is behind the sampled depth (i.e. inside geometry), 
		// it contributes to the occlusion factor
		// 在片段后面的 采样片段提供遮蔽因子
		// 采用的是右手坐标系
        occlusion += (sampleDepth >= sample.z + bias ? 1.0 : 0.0) * rangeCheck;// 照亮部分*rangeCheck
        //occlusion += (sampleDepth <= sample.z + bias ? 1.0 : 0.0) * rangeCheck;// 照亮部分*rangeCheck
		// depthMap值 >= 当前fragDepth+bias ? 1: 0
    }
    //occlusion =  (occlusion / kernelSize); // 阴影部分是白色，照亮部分是黑色
    occlusion = 1.0 - (occlusion / kernelSize); // 
    
    FragColor = vec4(occlusion,occlusion,occlusion,1);
}