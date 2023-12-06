using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using SistemaVenta.BLL.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepositorio<DetalleVenta> _detalleVentaRepositorio;
        private readonly IGenericRepositorio<Venta> _ventaRepositorioTable;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepositorio, IGenericRepositorio<Venta> ventaRepositorioTable, IGenericRepositorio<DetalleVenta> detalleVentaRepositorio, IMapper mapper)
        {
            _ventaRepositorioTable = ventaRepositorioTable;
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _mapper = mapper;
        }

        public async Task<List<VentaDTO>> Lista()
        {
            try
            {
                var queryUsuario = await _ventaRepositorioTable.Consultar();

                return _mapper.Map<List<VentaDTO>>(queryUsuario);
            }
            catch
            {
                throw;
            }
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepositorio.Registrar(_mapper.Map<Venta>(modelo));

                if (ventaGenerada.IdVenta == 0)
                    throw new TaskCanceledException("No se pudo crear");

                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar();
            var ListaResultado = new List<Venta>();

            try
            {
                if(buscarPor == "fecha")
                {
                    DateTime fInicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                    DateTime fFin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

                    ListaResultado = await query.Where(v =>
                        v.FechaRegistro.Value >= fInicio.Date &&
                        v.FechaRegistro.Value <= fFin.Date
                    )
                    .Include(dv => 
                        dv.DetalleVenta
                    )
                    .ThenInclude(p => 
                        p.IdProductoNavigation
                    )
                    .ToListAsync();
                }
                else
                {
                    ListaResultado = await query.Where(v =>
                        v.NumeroDocumento == numeroVenta
                    )
                    .Include(dv =>
                        dv.DetalleVenta
                    )
                    .ThenInclude(p =>
                        p.IdProductoNavigation
                    )
                    .ToListAsync();
                }
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<VentaDTO>>(ListaResultado);
        }

        public async Task<List<ReporteDTO>> reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Consultar();
            var ListaResultado = new List<DetalleVenta>();

            try
            {
                DateTime fInicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                DateTime fFin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

                ListaResultado = await query
                    .Include(p =>
                        p.IdProductoNavigation
                    )
                    .Include(v =>
                        v.IdVentaNavigation
                    )
                    .Where(dv =>
                        dv.IdVentaNavigation.FechaRegistro.Value >= fInicio.Date &&
                        dv.IdVentaNavigation.FechaRegistro.Value <= fFin.Date
                    )
                    .ToListAsync();
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<ReporteDTO>>(ListaResultado);
        }
    }
}
