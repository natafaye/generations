using System;

namespace Generations
{
    public static class BuildEvents
    {
        // When recipes are first loaded in or new recipes are added
        public static Action<StructureType[]> RecipesUpdated;

        // When the selected type for building is changed
        public static Action<StructureType> SelectedTypeChanged;
    }
}