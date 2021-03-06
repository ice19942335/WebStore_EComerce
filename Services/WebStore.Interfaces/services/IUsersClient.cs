﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using WebStore.Entities.Entities.Identity;

namespace WebStore.Interfaces.services
{
    public interface IUsersClient : IUserRoleStore<User>,
                                    IUserClaimStore<User>,
                                    IUserPasswordStore<User>,
                                    IUserTwoFactorStore<User>,
                                    IUserEmailStore<User>,
                                    IUserPhoneNumberStore<User>,
                                    IUserLoginStore<User>,
                                    IUserLockoutStore<User> { }
}
