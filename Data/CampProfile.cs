using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>() //domyslna mapa
            .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName))
            .ReverseMap(); //mapowanie pola specyficznie

            this.CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t=>t.Camp,opt=>opt.Ignore()) // ignoruje mapowanie dla Camp jest po reverse d;latego obowiazuje tylko przy mapowaniu Talk->TalkModel
                .ForMember(t => t.Speaker, opt => opt.Ignore()); 

            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
        }
    }
}
