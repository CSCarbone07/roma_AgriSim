using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchGaussianDistribution : GaussianDistribution
{
    private int patch_size_mean = 20;
    private int patch_size_std = 10;
    private float patch_position_std_mean = 1.0f;
    private float patch_position_std_std = 1.0f;
    private PlantPatch current_patch;
    private int i = 0;

    public PatchGaussianDistribution(int? seed = null) : base(seed)
    {
        patch_size_mean = 0;
        patch_size_std = 0;
        patch_position_std_mean = 0;
        patch_position_std_std = 0;
        
        createNewPatch();
    }

    public PatchGaussianDistribution(
        int patch_size_mean, 
        int patch_size_std, 
        float patch_position_std_mean, 
        float patch_position_std_std, 
        int? seed = null
    ) : base(seed)
    {
        this.patch_size_mean = patch_size_mean;
        this.patch_size_std = patch_size_std;
        this.patch_position_std_mean = patch_position_std_mean;
        this.patch_position_std_std = patch_position_std_std;
        
        createNewPatch();
    }

    public void createNewPatch()
    {
        // Get the extreme values for the field
        Vector4 field_dimensions = getFieldMinMax();

        // Get the patch size and the patch position std from the Gaussian distribution
        float patch_size = NextGaussian(patch_size_mean, patch_size_std);
        float position_std_x = NextGaussian(patch_position_std_mean, patch_position_std_std);
        float position_std_y = NextGaussian(patch_position_std_mean, patch_position_std_std);

        /*
        Debug.Log("new patch has size, std_x and std_y " + patch_size + ", " + position_std_x + ", " + position_std_y );
        Debug.Log("new patch has size mean and std " + patch_size_mean + ", " + patch_size_std);

        Debug.Log("new patch has pos mean and std " + patch_position_std_mean + ", " + patch_position_std_std );
        Debug.Log("new patch field limits " + field_dimensions[0] + ", " + field_dimensions[1] + ", " + field_dimensions[2] + ", " + field_dimensions[3] );
        */

        current_patch = new PlantPatch
        {
            size=(int)patch_size,
            position_mean = new Vector2(
                UnityEngine.Random.Range(field_dimensions[0], field_dimensions[1]), 
                UnityEngine.Random.Range(field_dimensions[2], field_dimensions[3])
            ),
            position_std = new Vector2(position_std_x, position_std_y)
        };

    }

    public override Vector2 getNextPosition() 
    {
        //Debug.Log("getting next position");

        // Recalculate the current patch position
        if (i >= current_patch.size) {
            createNewPatch();
            i = 0;
        }

        // Get the extreme values for the field
        Vector4 field_dimensions = getFieldMinMax();

        // Get x and y coordinates
        float x = NextGaussian(current_patch.position_mean[0], current_patch.position_std[0], field_dimensions[0], field_dimensions[1]);
        float y = NextGaussian(current_patch.position_mean[1], current_patch.position_std[1], field_dimensions[2], field_dimensions[3]);
        Vector2 gaussian_position = new Vector2(x, y);

        // Add to the list
        //generated_positions.Add(gaussian_position);

        i++;

        return gaussian_position;
    }
}


[System.Serializable]
public class PlantPatch{
    public int size;
    public Vector2 position_mean;
    public Vector2 position_std;
}