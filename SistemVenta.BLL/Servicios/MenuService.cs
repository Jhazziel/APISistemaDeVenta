﻿using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using SistemaVenta.BLL.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Servicios
{
    public class MenuService : IMenuService
    {
        IGenericRepositorio<Usuario> _usuarioRepositorio;
        IGenericRepositorio<MenuRol> _menuRolRepositorio;
        IGenericRepositorio<Menu> _menuRepositorio;
        private readonly IMapper _mapper;

        public MenuService(IGenericRepositorio<Usuario> usuarioRepositorio, IGenericRepositorio<MenuRol> menuRolRepositorio, IGenericRepositorio<Menu> menuRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _menuRolRepositorio = menuRolRepositorio;
            _menuRepositorio = menuRepositorio;
            _mapper = mapper;
        }

        public async Task<List<MenuDTO>> Lista(int idUsuario)
        {
            IQueryable<Usuario> tablaUsuario = await _usuarioRepositorio.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<MenuRol> tablaMenuRol = await _menuRolRepositorio.Consultar();
            IQueryable<Menu> tablaMenu = await _menuRepositorio.Consultar();

            try
            {
                IQueryable<Menu> tablaResultado = (from u in tablaUsuario
                                                   join mr in tablaMenuRol on u.IdRol equals mr.IdRol
                                                   join m in tablaMenu on mr.IdMenu equals m.IdMenu
                                                   select m).AsQueryable();

                var listaMenu = tablaResultado.ToList();

                return _mapper.Map<List<MenuDTO>>(listaMenu);
            }
            catch
            {
                throw;
            }
        }
    }
}
