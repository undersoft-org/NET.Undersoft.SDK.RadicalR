using System;
using System.Collections.Generic;

namespace RadicalR
{
    public interface IRepositoryEvents
    {
       IDeputy OnAdding { get; set; }
       IDeputy OnAddComplete { get; set; }
       IDeputy OnGetting { get; set; }
       IDeputy OnGetComplete { get; set; }
       IDeputy OnSetting { get; set; }
       IDeputy OnSetComplete { get; set; }
       IDeputy OnDeleting { get; set; }
       IDeputy OnDeleteComplete { get; set; }
       IDeputy OnSaving { get; set; }
       IDeputy OnSaveComplete { get; set; }
       IDeputy OnFiltering { get; set; }
       IDeputy OnFilterComplete { get; set; }
       IDeputy OnFinding { get; set; }
       IDeputy OnFindComplete { get; set; }
       IDeputy OnMapping { get; set; }
       IDeputy OnMapComplete { get; set; }
       IDeputy OnExist { get; set; }
       IDeputy OnExistComplete { get; set; }
       IDeputy OnNonExist { get; set; }
       IDeputy OnNonExistComplete { get; set; }
       IDeputy OnValidating { get; set; }
       IDeputy OnValidateComplete { get; set; }
    }

  
}