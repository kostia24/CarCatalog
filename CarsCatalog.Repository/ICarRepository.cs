using System;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface ICarRepository
    {
        Car GetCarById(int? id);
        IQueryable<Car> GetCarsByUserId(string userId);
        IQueryable<Car> GetCarsByModelId(int? modelId);
        void Add(Car car);
        void Update(Car updatedCar);
        void Delete(Car car);
    }
}