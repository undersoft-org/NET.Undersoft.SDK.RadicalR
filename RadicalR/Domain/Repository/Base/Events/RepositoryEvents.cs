using System;
using System.Linq;
using System.Series;
using System.Threading.Tasks;

namespace RadicalR
{
    public class EventDeputy : Deputy, IDeputy
    {
        public EventDeputy(StateOn eventon) : base()
        {
            StateOn = eventon;
            RepositoryEvents.Registry.Put("On" + eventon.ToString(), this);
        }
        public EventDeputy(string eventon) : base()
        {
            StateOn = Enum.Parse<StateOn>(eventon);
            RepositoryEvents.Registry.Put("On" + eventon, this);
        }

        public override Task Publish(params object[] parameters)
        {
            if (RepositoryEvents.Registry.TryGet("On" + StateOn.ToString(), out ICard<IDeputy> card))
                return card.ForEachAsync((c) => { c.Publish(true, TargetObject, parameters); });
            return null;
        }

        public override Task Execute(params object[] parameters)
        {
            if (RepositoryEvents.Registry.TryGet("On" + StateOn.ToString(), out ICard<IDeputy> card))
                return card.ForEachAsync((c) => { c.Publish(true, TargetObject, parameters); });
            return null;
        }
    }

    public class RepositoryEvents : IRepositoryEvents
    {
        public static IDeck<IDeputy> Registry;

        static RepositoryEvents()
        {
            Registry = new Catalog<IDeputy>(true);
            var states = Enum.GetNames<StateOn>();
            var init = new RepositoryEvents();
            states.ForEach((s) => Registry.Add(s.ToString(),
                new EventDeputy(s))).CommitAsync();
        }

        private IDeputy GetPublisher(string eventonName)
        {
            var deputy = Registry.Get(eventonName);
            deputy.TargetObject = this;
            return deputy;
        }

        public IDeputy OnAdding
        {
            get => GetPublisher(nameof(StateOn.Adding));
            set => Registry.Add(nameof(OnAdding), value);
        }
        public IDeputy OnAddComplete
        {
            get => GetPublisher(nameof(StateOn.AddComplete));
            set { Registry.Add(nameof(OnAddComplete), value); }
        }

        public IDeputy OnGetting
        {
            get => GetPublisher(nameof(StateOn.Getting));
            set { Registry.Add(nameof(OnGetting), value); }
        }
        public IDeputy OnGetComplete
        {
            get => GetPublisher(nameof(StateOn.GetComplete));
            set { Registry.Add(nameof(OnGetComplete), value); }
        }

        public IDeputy OnPatching
        {
            get => GetPublisher(nameof(StateOn.Patching));
            set { Registry.Add(nameof(OnPatching), value); }
        }
        public IDeputy OnPatchComplete
        {
            get => GetPublisher(nameof(StateOn.PatchComplete));
            set { Registry.Add(nameof(OnPatchComplete), value); }
        }

        public IDeputy OnUpsert
        {
            get => GetPublisher(nameof(StateOn.Upsert));
            set { Registry.Add(nameof(OnUpsert), value); }
        }
        public IDeputy OnUpsertComplete
        {
            get => GetPublisher(nameof(StateOn.UpsertComplete));
            set { Registry.Add(nameof(OnUpsertComplete), value); }
        }

        public IDeputy OnSetting
        {
            get => GetPublisher(nameof(StateOn.Setting));
            set { Registry.Add(nameof(OnSetting), value); }
        }
        public IDeputy OnSetComplete
        {
            get => GetPublisher(nameof(StateOn.SetComplete));
            set { Registry.Add(nameof(OnSetComplete), value); }
        }

        public IDeputy OnDeleting
        {
            get => GetPublisher(nameof(StateOn.Deleting));
            set { Registry.Add(nameof(OnDeleting), value); }
        }
        public IDeputy OnDeleteComplete
        {
            get => GetPublisher(nameof(StateOn.DeleteComplete));
            set { Registry.Add(nameof(OnDeleteComplete), value); }
        }

        public IDeputy OnSaving
        {
            get => GetPublisher(nameof(StateOn.Saving));
            set { Registry.Add(nameof(OnSaving), value); }
        }
        public IDeputy OnSaveComplete
        {
            get => GetPublisher(nameof(StateOn.SaveComplete));
            set { Registry.Add(nameof(OnSaveComplete), value); }
        }

        public IDeputy OnFiltering
        {
            get => GetPublisher(nameof(StateOn.Filtering));
            set { Registry.Add(nameof(OnFiltering), value); }
        }
        public IDeputy OnFilterComplete
        {
            get => GetPublisher(nameof(StateOn.FilterComplete));
            set { Registry.Add(nameof(OnFilterComplete), value); }
        }

        public IDeputy OnFinding
        {
            get => GetPublisher(nameof(StateOn.Finding));
            set { Registry.Add(nameof(OnFinding), value); }
        }
        public IDeputy OnFindComplete
        {
            get => GetPublisher(nameof(StateOn.FindComplete));
            set { Registry.Add(nameof(OnFindComplete), value); }
        }

        public IDeputy OnMapping
        {
            get => GetPublisher(nameof(StateOn.Mapping));
            set { Registry.Add(nameof(OnMapping), value); }
        }
        public IDeputy OnMapComplete
        {
            get => GetPublisher(nameof(StateOn.MapComplete));
            set { Registry.Add(nameof(OnMapComplete), value); }
        }

        public IDeputy OnExist
        {
            get => GetPublisher(nameof(StateOn.Exist));
            set { Registry.Add(nameof(OnExist), value); }
        }
        public IDeputy OnExistComplete
        {
            get => GetPublisher(nameof(StateOn.ExistComplete));
            set { Registry.Add(nameof(OnExistComplete), value); }
        }

        public IDeputy OnNonExist
        {
            get => GetPublisher(nameof(StateOn.NonExist));
            set { Registry.Add(nameof(OnNonExist), value); }
        }
        public IDeputy OnNonExistComplete
        {
            get => GetPublisher(nameof(StateOn.NonExistComplete));
            set { Registry.Add(nameof(OnNonExistComplete), value); }
        }

        public IDeputy OnValidating
        {
            get => GetPublisher(nameof(StateOn.Validating));
            set { Registry.Add(nameof(OnValidating), value); }
        }
        public IDeputy OnValidateComplete
        {
            get => GetPublisher(nameof(StateOn.ValidateComplete));
            set { Registry.Add(nameof(OnValidateComplete), value); }
        }
    }


}
