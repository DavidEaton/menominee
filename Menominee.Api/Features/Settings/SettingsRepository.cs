﻿using Menominee.Api.Data;
using Menominee.Domain.Entities.Settings;
using Menominee.Shared.Models.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Menominee.Api.Features.Settings
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly ApplicationDbContext context;

        public SettingsRepository(
            ApplicationDbContext context)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get all of the configuration settings
        /// </summary>
        /// <returns>list of settings</returns>
        public async Task<IReadOnlyList<SettingToRead>> GetListAsync()
        {
            var allSettings = await context.Settings
                .AsNoTracking()
                .ToListAsync();

            return allSettings
                 .Select(setting => SettingHelper.ConvertToReadDto(setting))
                 .ToList();
        }

        /// <summary>
        /// Gets a single setting from the database using the settings name
        /// </summary>
        /// <param name="settingName">The name of the setting to grab</param>
        /// <returns>a single SettignToRead dto from the database with the setting name</returns>
        public async Task<SettingToRead> GetAsync(SettingName settingName)
        {
            var setting = await context.Settings.Where(x => x.SettingName.Equals(settingName)).AsNoTracking().FirstOrDefaultAsync();

            return SettingHelper.ConvertToReadDto(setting);
        }

        /// <summary>
        /// Get all setting relating to a specified group
        /// </summary>
        /// <param name="groupId">The setting group</param>
        /// <returns>list of settings</returns>
        public async Task<IReadOnlyList<SettingToRead>> GetByGroupAsync(SettingGroup group)
        {
            var settings = await context.Settings.Where(setting => setting.SettingGroup.Equals(group)).AsNoTracking().ToListAsync();

            return settings
                .Select(setting => SettingHelper.ConvertToReadDto(setting))
                .ToList();
        }

        /// <summary>
        /// bulk save a list of settings
        /// </summary>
        /// <param name="settings">list of settings</param>
        /// <returns>list of settings to read</returns>
        public async Task<IReadOnlyList<SettingToRead>> SaveListAsync(List<SettingToWrite> settings)
        {
            var settingsToUpdate = new List<ConfigurationSetting>();
            if (settings is not null)
                settings.ForEach(setting =>
                {
                    var settingExists = context.Settings.Where(x => x.SettingName.Equals(setting.SettingName)).FirstOrDefault();

                    if (settingExists != default || settingExists != null)
                    {
                        settingExists.SetProperties(settingExists.SettingName, settingExists.SettingGroup, settingExists.SettingValueType, setting.SettingValue);
                    }
                    else
                    {
                        settingsToUpdate.Add(SettingHelper.ConvertWriteDtoToEntity(setting));
                    }
                });

            if (settingsToUpdate.Any())
                context.Settings.AddRange(settingsToUpdate);

            await SaveChangesAsync();

            var getSettings = await context.Settings.AsNoTracking().ToListAsync();

            return getSettings.Select(setting => SettingHelper.ConvertToReadDto(setting)).ToList();
        }

        /// <summary>
        /// Bulk update a list of settings
        /// </summary>
        /// <param name="settings">a list of settings</param>
        /// <returns>read list of settings</returns>
        public async Task<IReadOnlyList<SettingToRead>> UpdateListAsync(List<SettingToWrite> settings)
        {
            if (settings is not null)
            {
                settings.ForEach(setting =>
                {
                    var settingToUpdate = context.Settings
                        .Where(settingToUpdate => settingToUpdate.SettingName == setting.SettingName).FirstOrDefault();
                    if (settingToUpdate != default && settingToUpdate.SettingName == setting.SettingName)
                        settingToUpdate.SetProperties(settingToUpdate.SettingName, settingToUpdate.SettingGroup, settingToUpdate.SettingValueType, setting.SettingValue);
                });

                await SaveChangesAsync();
            }

            var getSettings = await context.Settings.AsNoTracking().ToListAsync();

            return getSettings.Select(setting => SettingHelper.ConvertToReadDto(setting)).ToList();
        }

        /// <summary>
        /// Updates a single setting in the databse
        /// </summary>
        /// <param name="setting">a single setting</param>
        /// <returns>A single SettingToRead dto of the updated setting</returns>
        public async Task<SettingToRead> UpdateAsync(SettingToWrite setting)
        {
            if (setting is not null)
            {
                var updateSetting = context.Settings.Where(x => x.SettingName.Equals(setting.SettingName)).FirstOrDefault();
                if (updateSetting != default && updateSetting.SettingName == setting.SettingName)
                    updateSetting.SetProperties(updateSetting.SettingName, updateSetting.SettingGroup, updateSetting.SettingValueType, setting.SettingValue);
            }

            await SaveChangesAsync();

            return SettingHelper.ConvertToReadDto(await context.Settings.FirstOrDefaultAsync(x => x.SettingName.Equals(setting.SettingName)));
        }

        /// <summary>
        /// Saves a single setting in the database 
        /// </summary>
        /// <param name="setting">a single setting</param>
        /// <returns>A single SettingToRead dto on the newly saved setting</returns>
        public async Task<SettingToRead> SaveAsync(SettingToWrite setting)
        {
            if (setting is not null)
            {
                var settingExists = context.Settings.Where(x => x.SettingName.Equals(setting.SettingName)).FirstOrDefault();

                if (settingExists != default)
                {
                    settingExists.SetProperties(settingExists.SettingName, settingExists.SettingGroup, settingExists.SettingValueType, setting.SettingValue);
                }
                else
                {
                    context.Settings.Add(SettingHelper.ConvertWriteDtoToEntity(setting));
                }
            }

            await SaveChangesAsync();

            return SettingHelper.ConvertToReadDto(await context.Settings.FirstOrDefaultAsync(x => x.SettingName.Equals(setting.SettingName)));
        }

        public async Task<ConfigurationSetting> GetEntityAsync(long id)
        {
            return await context.Settings
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public void Add(ConfigurationSetting entity)
        {
            if (entity is not null)
                context.Settings.Attach(entity);
        }

        /// <summary>
        /// Save changes to Database
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

    }
}
