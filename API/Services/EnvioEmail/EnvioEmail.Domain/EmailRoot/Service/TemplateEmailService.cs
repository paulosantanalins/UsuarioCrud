using EnvioEmail.Domain.EmailRoot.Dto;
using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Repository;
using EnvioEmail.Domain.EmailRoot.Service.Interfaces;
using EnvioEmail.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Utils.Base;

namespace EnvioEmail.Domain.EmailRoot.Service
{
    public class TemplateEmailService : ITemplateEmailService
    {
        private readonly ITemplateEmailRepository _templateEmailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TemplateEmailService(ITemplateEmailRepository templateEmailRepository, IUnitOfWork unitOfWork)
        {
            _templateEmailRepository = templateEmailRepository;
            _unitOfWork = unitOfWork;
        }


        public FiltroGenericoDtoBase<TemplateEmailDto> Filtrar(FiltroGenericoDtoBase<TemplateEmailDto> filtro)
        {
            return _templateEmailRepository.FiltrarTemplatesEmail(filtro);
        }

        public TemplateEmail BuscarPorId(int id)
        {
            var templateEmail = _templateEmailRepository.BuscarPorId(id);
            return templateEmail;
        }

        public TemplateEmail BuscarPorIdComParametors(int id)
        {
            var templateEmail = _templateEmailRepository.BuscarPorIdComParametros(id);
            return templateEmail;
        }

        public void PersistirTemplateEmail(TemplateEmail templateEmail)
        {
            if (templateEmail.FlagCorpoAlterado)
            {
                string aux = "";

                for (int i = 0; i < templateEmail.Corpo.Length; i++)
                {
                    if (templateEmail.Corpo[i] == '[')
                    {

                        for (int j = i; j < templateEmail.Corpo.Length; j++)
                        {
                            aux += templateEmail.Corpo[j];
                            if (templateEmail.Corpo[j] == ']' && templateEmail.Corpo[j + 1] != ']')
                            {
                                if ((templateEmail.Parametros as List<ParametroTemplate>).Find(x => x.NomeParametro == aux) == null)
                                {
                                    (templateEmail.Parametros as List<ParametroTemplate>).Add(new ParametroTemplate { NomeParametro = aux });
                                }
                                aux = "";
                                i = j;
                                break;
                            }
                        }
                    }
                }

                templateEmail.Corpo += @"<style>.html {    border: 1px solid #ddd;    border-radius: 4px;    padding: 0.5rem;    background-color: #f1f1f1;    min-height: 20px;    max-height: 10rem;    overflow: auto;  }       .ql-font-serif {      font-family: Georgia, Times New Roman, serif;    }    .ql-font-monospace {      font-family: Monaco, Courier New, monospace;    }    .ql-size-small {      font-size: 0.75em;    }    .ql-size-large {      font-size: 1.5em;    }    .ql-size-huge {      font-size: 2.5em;    }    .ql-direction-rtl {      direction: rtl;      text-align: inherit;    }    .ql-align-center {      text-align: center;    }    .ql-align-justify {      text-align: justify;    }    .ql-align-right {      text-align: right;    }    blockquote {      border-left: 4px solid #ccc;      padding-left: 16px;    }    code,    pre {      background-color: #f0f0f0;      border-radius: 3px;      padding: 6px 10px;    }    ul>li[data-checked=true]::before {      content: '\2611';    }    ul>li[data-checked=false]::before {      content: '\2610';    }    ol>li,    ul>li {      list-style-type: none;    }    ol {      counter-reset: mylist    }    ol>li:before {      counter-increment: mylist;      content: counter(mylist, decimal) '. ';    }    ol ol>li:before {      content: counter(mylist, lower-alpha) '. ';    }    ol ol ol>li:before {      content: counter(mylist, lower-roman) '. ';    }    ol ol ol ol>li:before {      content: counter(mylist, decimal) '. ';    }    ol ol ol ol ol>li:before {      content: counter(mylist, lower-alpha) '. ';    }    ol ol ol ol ol ol>li:before {      content: counter(mylist, lower-roman) '. ';    }    ol ol ol ol ol ol ol>li:before {      content: counter(mylist, decimal) '. ';    }    ol ol ol ol ol ol ol ol>li:before {      content: counter(mylist, lower-alpha) '. ';    }    ol ol ol ol ol ol ol ol ol>li:before {      content: counter(mylist, lower-roman) '. ';    }    /* ql indent */    .ql-indent-1:not(.ql-direction-rtl) {      padding-left: 3em;    }    .ql-indent-1.ql-direction-rtl.ql-align-right {      padding-right: 3em;    }    .ql-indent-2:not(.ql-direction-rtl) {      padding-left: 6em;    }    .ql-indent-2.ql-direction-rtl.ql-align-right {      padding-right: 6em;    }    .ql-indent-3:not(.ql-direction-rtl) {      padding-left: 9em;    }    .ql-indent-3.ql-direction-rtl.ql-align-right {      padding-right: 9em;    }    .ql-indent-4:not(.ql-direction-rtl) {      padding-left: 12em;    }    .ql-indent-4.ql-direction-rtl.ql-align-right {      padding-right: 12em;    }    .ql-indent-5:not(.ql-direction-rtl) {      padding-left: 15em;    }    .ql-indent-5.ql-direction-rtl.ql-align-right {      padding-right: 15em;    }    .ql-indent-6:not(.ql-direction-rtl) {      padding-left: 18em;    }    .ql-indent-6.ql-direction-rtl.ql-align-right {      padding-right: 18em;    }    .ql-indent-7:not(.ql-direction-rtl) {      padding-left: 21em;    }    .ql-indent-7.ql-direction-rtl.ql-align-right {      padding-right: 21em;    }    .ql-indent-8:not(.ql-direction-rtl) {      padding-left: 24em;    }    .ql-indent-8.ql-direction-rtl.ql-align-right {      padding-right: 24em;    }    .ql-indent-9:not(.ql-direction-rtl) {      padding-left: 27em;    }    .ql-indent-9.ql-direction-rtl.ql-align-right {      padding-right: 27em;    }    /* video */    .ql-video {      display: block;      max-width: 100%;    }    .ql-video.ql-align-center {      margin: 0 auto;    }    .ql-video.ql-align-right {      margin: 0 0 0 auto;    }</style>";
            }

            if (templateEmail.Id == 0)
            {

                _templateEmailRepository.AdicionarTemplateEmail(templateEmail);
                _unitOfWork.Commit();
            }
            else
            {
                _templateEmailRepository.UpdateComParametro(templateEmail);
                _unitOfWork.Commit();
            }
        }
    }
}
