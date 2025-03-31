// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public interface ILoaiRepository

    {
        Task<IEnumerable<Loai>> GetAllAsync();
        Task<Loai> GetByIdAsync(string id);
        Task AddAsync(Loai loai);
        Task UpdateAsync(Loai loai);
        Task DeleteAsync(string id);
    }
}
