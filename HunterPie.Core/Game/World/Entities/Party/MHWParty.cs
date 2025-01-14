using HunterPie.Core.Domain.Interfaces;
using HunterPie.Core.Extensions;
using HunterPie.Core.Game.Client;
using HunterPie.Core.Game.World.Definitions;
using HunterPie.Core.Native.IPC.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterPie.Core.Game.World.Entities.Party;

public class MHWParty : IParty, IEventDispatcher
{
    public const int MAXIMUM_SIZE = 4;

    private readonly Dictionary<long, MHWPartyMember> _partyMembers = new(MAXIMUM_SIZE);

    public int Size
    {
        get
        {
            lock (_partyMembers)
                return _partyMembers.Count;
        }
    }

    public int MaxSize => MAXIMUM_SIZE;

    public List<IPartyMember> Members
    {
        get
        {
            lock (_partyMembers)
                return _partyMembers.Values.ToList<IPartyMember>();
        }
    }

    public event EventHandler<IPartyMember> OnMemberJoin;
    public event EventHandler<IPartyMember> OnMemberLeave;

    public void Update(long memberAddress, MHWPartyMemberData data)
    {
        lock (_partyMembers)
        {
            if (!_partyMembers.TryGetValue(memberAddress, out MHWPartyMember member))
            {
                Add(memberAddress, data);
                return;
            }

            ((IUpdatable<MHWPartyMemberData>)member).Update(data);
        }
    }

    public void Update(long memberAddress, EntityDamageData data)
    {
        lock (_partyMembers)
        {
            if (!_partyMembers.TryGetValue(memberAddress, out MHWPartyMember member))
                return;

            ((IUpdatable<EntityDamageData>)member).Update(data);
        }
    }

    public void Add(long memberAddress, MHWPartyMemberData data)
    {
        var member = new MHWPartyMember();
        ((IUpdatable<MHWPartyMemberData>)member).Update(data);
        _partyMembers.Add(memberAddress, member);

        this.Dispatch(OnMemberJoin, member);
    }

    public void Remove(long memberAddress)
    {
        MHWPartyMember member;
        lock (_partyMembers)
        {
            if (!_partyMembers.Remove(memberAddress, out member))
                return;
        }

        this.Dispatch(OnMemberLeave, member);
    }
}
