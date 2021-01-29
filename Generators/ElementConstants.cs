using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    public static class ElementConstants
    {
        //ELEMENT CONSTANTS

        //In my opinion, these produce varied and high quality output, so I do not recommend
        //editting them. However, if the time ever comes:
        //Take great care and pay special attention to bounds!

        //Game breaking ones are marked with a "!". Unmarked ones can be editted freely, but will DRASTICALLY
        //change the results of element generation.

        //side note: added a min and max for each field

        public const double normalized_min = 0.0; //literally don't touch this
        public const double normalized_max = 1.0; //or this

        public const int covalence_max = 8; //!!!
        public const int covalence_min = 1; //!!!

        public const int catenation_min = 0; //!
        public const int catenation_max = 4;

        public const int is_cubic = 0; //!
        public const int is_crystal = 1; //!

        public const int is_body_centered = 1; //!
        public const int is_face_centered = 2; //!
        public const int is_hexagonal_cuboid = 3; //!

        public const double structure_cubic_limit = 0.95;

        public const double deformation_max = 0.5;
        public const double deformation_min = 0.0;
        public const double deformation_even_symmetry_multiplier = 0.1;
        public const double deformation_odd_symmetry_multiplier = -0.075;

        public const double density_even_symmetry_multiplier = 0.75;
        public const double density_odd_symmetry_multiplier = 0.5;
        public const int density_max = 25; //!
        public const int density_min = 1; //!

        public const int geometry_max = 12; //!
        public const int geometry_min = 1; //!
        public const double geometry_add_op = 1.25; //!
        public const double geometry_sub_op = 1.6; //!

        public const int hardness_max = 10; //!
        public const int hardness_min = 1; //!
        public const int hardness_cubic_damp_limit = 2;
        public const int hardness_crystalline_damp_limit = 3;

        public const double pliance_hard_cubics_max = 0.45;
        public const double pliance_hard_cubics_min = 0.2;
        public const double pliance_soft_cubics_min = 0.1;
        public const double pliance_soft_cubics_max = 0.25;
        public const double pliance_crystalline_min = 0.0;
        public const double pliance_crystalline_max = 0.2;
        public const double pliance_min = 0.0;
        public const double pliance_max = 1.0;

        //these aren't marked, but gravity is not bounded.
        //because the value is a percentage, it doesn't make sense
        //to go over 1. So bear that in mind
        public const double gravity_cubic_base_weight = 0.05;
        public const double gravity_crystalline_base_weight = 0.0;
        public const double gravity_cubic_iter_max = 0.0175;
        public const double gravity_crystalline_iter_max = 0.02;
        public const double gravity_iter_min = 0.01;
        public const double gravity_min = 0.01; //theoretical minimum, highly unlikely
        public const double gravity_max = 0.4875; //theoretical maximum, highly unlikely

        //these  aren't marked, but bear in mind that these
        //are multipliers! Melting point and boiling point
        //will drastically increase or increase when editted!
        public const double heat_res_body_centered_min = 1.1;
        public const double heat_res_body_centered_max = 1.4;
        public const double heat_res_face_centered_min = 0.75;
        public const double heat_res_face_centered_max = 1.2;
        public const double heat_res_hexagonal_cuboid_min = 1.15;
        public const double heat_res_hexagonal_cuboid_max = 1.9;
        public const double heat_res_abnormal_cuboid_min = 0.3;
        public const double heat_res_abnormal_cuboid_max = 0.5;
        public const double heat_res_non_zero_boiling_scaler = 2.0;
        public const double heat_res_zero_boiling_scaler = 1.25;

        public const double pressure_damp_min = 0.0;
        public const double pressure_damp_max = 0.15;

        public const double melting_point_density_scaler = 50.0;
        public const double boiling_point_density_scaler = 100.0;

        public const double cleave_cubic_min = 0.0;
        public const double cleave_cubic_max = 0.4;
        public const double cleave_crystal_min = 0.3;
        public const double cleave_crystal_max = 1.0;
        public const double cleave_crystal_damp = 2.2;
        public const double cleave_damp = 4.0;
        public const double cleave_min = 0.0;
        public const double cleave_max = 1.0;


    }
}
