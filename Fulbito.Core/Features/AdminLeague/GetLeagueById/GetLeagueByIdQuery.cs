using Fulbito.Core.Features.ViewLeague;

namespace Fulbito.Core.Features.AdminLeague.GetLeagueById;

public record GetLeagueByIdQuery(Guid LeagueId);

public record GetLeagueByIdResponse : ViewLeagueResponse;