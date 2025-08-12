using MC_BSR_S2_Calculator.Utility.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {

    // example with a starting dataset
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ListDisplayExampleStartingDataset : ListDisplay<DisplayClassExample>, IStorable {

        // the starting data set
        public override NotifyingList<DisplayClassExample> ClassDataList { get; set; } = new();

        // save location for this class
        public List<string> SaveLocation { get => ["list_display_example_dataset.json"]; }

        // as IStorable reference
        public IStorable AsIStorable { get => ((IStorable)this); }

        // parametereless constructo for XAML usage
        public ListDisplayExampleStartingDataset() 
            : base() { }

        // parametered constructor for class duplication
        public ListDisplayExampleStartingDataset(NotifyingList<DisplayClassExample> classDataList) 
            : base() {
            ClassDataList = classDataList;
        }

        // method to set the class data list and attempt to load
        protected override void SetClassDataList() {
            // default dataset
            ClassDataList.AddRange(
                new(),
                new(),
                new(),
                new(),
                new()
            );

            // try to load data
            AsIStorable.TryLoad(new ListDisplayExampleStartingDataset(ClassDataList));
        }

        // value mirroring method
        public void MirrorValues<T>(T cls)
            where T : class, IStorable {
            if (cls is ListDisplayExampleStartingDataset clsCasted) {
                ClassDataList = clsCasted.ClassDataList;
            }
        }

        // load as functionality for this class
        public void LoadAs() => AsIStorable.Load<ListDisplayExampleStartingDataset>();
    }

    // example with no starting dataset
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ListDisplayExampleBlank : ListDisplay<DisplayClassExample>, IStorable {
        // save location for this class
        public List<string> SaveLocation { get => ["list_display_example_dataset.json"]; }

        // as IStorable reference
        public IStorable AsIStorable { get => ((IStorable)this); }

        // method to set the class data list and attempt to load
        protected override void SetClassDataList() {
            // try to load data
            AsIStorable.TryLoad(new ListDisplayExampleBlank());
        }

        // value mirroring method
        public void MirrorValues<T>(T cls)
            where T : class, IStorable {
            if (cls is ListDisplayExampleBlank clsCasted) {
                ClassDataList = clsCasted.ClassDataList;
            }
        }

        // load as functionality for this class
        public void LoadAs() => AsIStorable.Load<ListDisplayExampleBlank>();
    }
}
