using AutoMapper;
using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PasswordService : IPasswordService
    {
        IMapper mapper;
        public PasswordService(IMapper mapperr)
        {
            mapper = mapperr;
        }

        public int getStrengthByPassword(string p)
        {

            var result = Zxcvbn.Core.EvaluatePassword(p);
           int strength= result.Score;
            return strength;
        }
    }
}