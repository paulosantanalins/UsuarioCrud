using Seguranca.Domain.Core.Notifications;
using Seguranca.Domain.UsuarioRoot.Repository;
using Seguranca.Domain.UsuarioRoot.Service.Interfaces;
using Seguranca.Domain.UsuarioRoot.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace Seguranca.Domain.UsuarioRoot.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly NotificationHandler _notificationHandler;

        public UsuarioService(
                              NotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
        }
      


        
    }
}
