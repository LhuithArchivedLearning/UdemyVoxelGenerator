using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public class Utils  {
	
	static int maxHeight = 150;
	static float smooth = 0.01f;
	static int octaves = 4;
	static float persistance = 0.5f;

	public static int GenerateStoneHeight(float x, float z){
		
		float height = Map(0, maxHeight-15, 0, 1, FBM(x*smooth*2,z*smooth*2,octaves+1,persistance));
		return (int) height;
	}

	public static int GenerateHeight(float x, float z){
		
		float height = Map(0, maxHeight, 0, 1, FBM(x*smooth,z*smooth,octaves,persistance));
		return (int) height;
	}

	static float Map(float newmin, float newmax, float omin, float omax, float value){
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(omin, omax, value));
    }

	
    public static float FBM3D(float x, float y, float z, float sm, int oct){

		float XY = FBM(x*sm, y*sm, oct, 0.5f);
		float YZ = FBM(y*sm, z*sm, oct, 0.5f);
		float XZ = FBM(x*sm, z*sm, oct, 0.5f);
		float YX = FBM(y*sm, x*sm, oct, 0.5f);
		float ZY = FBM(z*sm, y*sm, oct, 0.5f);
		float ZX = FBM(z*sm, x*sm, oct, 0.5f);

        return (XY+YZ+XZ+YX+ZY+ZX)/6.0f;
    }

    static float FBM(float x, float z, int octaves, float persistance){

        float total = 0;
        float frequancy = 1;
        float amplitude = 1;
        float maxValue = 0;
		float offset = 32000f;
        for(int i = 0; i < octaves; i++){
            total += Mathf.PerlinNoise((x+offset) * frequancy, (z+offset) * frequancy) * amplitude;
            maxValue += amplitude;
            amplitude *= persistance;
            frequancy *= 2;
        }

        return total/maxValue;
    }
}
}

