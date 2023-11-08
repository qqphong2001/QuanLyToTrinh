using AutoMapper;
using Database.Models;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IFieldService
    {
        Task<FieldDTO> Create(FieldDTO payload);
        Task<IEnumerable<FieldDTO>> GetAll();
        Task<FieldDTO> GetById(int id);
        Task<FieldDTO> Update(FieldDTO payload);
        Task<int> Delete(int id);

    }

    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _repository;
        private readonly IMapper _mapper;
        public FieldService(IFieldRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }
        public async Task<FieldDTO> Create(FieldDTO payload)
        {
           
            var data = _mapper.Map<TblField>(payload);
            try
            {
                await _repository.AddAsync(data);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<FieldDTO>(data);
        }

        public async Task<int> Delete(int id)
        {
            var foundItem = _repository.FirstOrDefault(x => x.Id == id);
            if (foundItem == null)
            {
                throw new Exception("Item not found");
            }
            try
            {
                _repository.Remove(foundItem);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return id;
        }

        public async Task<IEnumerable<FieldDTO>> GetAll()
        {
            var data = _repository.GetAll();
            return _mapper.Map<IEnumerable<FieldDTO>>(data);
        }

        public async Task<FieldDTO> GetById(int id)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                throw new Exception("Item not found");
            }
            return _mapper.Map<FieldDTO>(data);
        }

        public async Task<FieldDTO> Update(FieldDTO payload)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
            if (data == null)
            {
                throw new Exception($"{payload.Id} was not found");
            }
            data.Title = payload.Title;
            data.Active = payload.Active;
            await _repository.SaveChanges();
            return _mapper.Map<FieldDTO>(data);
        }
    }
}
