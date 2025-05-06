using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Application.Base
{
    public class ApplicationBase
    {
        protected readonly IMediator _mediator;
        protected readonly IMapper? _mapper;
        //protected readonly IUnitOfWork? _unitOfWork;

        public ApplicationBase(IMediator mediator)
            => _mediator = mediator;

        public ApplicationBase(IMediator mediator, IMapper mapper) : this(mediator)
            => _mapper = mapper;
        //public ApplicationBase(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork) : this(mediator, mapper)
        //    => _unitOfWork = unitOfWork;
    }
}
