using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Leaderboard
{
    public int BoardId { get; set; }

    public string Season { get; set; } = null!;

    public virtual ICollection<Ranking> Rankings { get; set; } = new List<Ranking>();
}
