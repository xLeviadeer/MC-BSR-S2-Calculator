using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Json;
using Newtonsoft.Json;

namespace MC_BSR_S2_Calculator.PlayerColumn {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class PlayerList : ListDisplay<Player>, IStorable {

        // --- VARIABLES ---

        // -- Storable --

        public List<string> SaveLocation { get => ["players.json"]; }

        public IStorable AsIStorable { get => ((IStorable)this); }

        // --- CONSTRUCTORS ---

        protected override void SetClassDataList() {
            AsIStorable.TryLoad(new PlayerList());
        }

        // --- METHODS ---

        // -- Storable --

        public void MirrorValues<U>(U cls)
            where U : class, IStorable {
            if (cls is PlayerList clsCasted) {
                ClassDataList = clsCasted.ClassDataList;
            }
        }

        public void LoadAs() => AsIStorable.Load<PlayerList>();
    }
}
