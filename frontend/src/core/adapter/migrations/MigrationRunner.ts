import semverCompare from 'semver/functions/compare';

import Logger from '../../services/logging/logImport';
import { Migration } from './Migration';

const allMigrations: Migration[] = [
  {
    name: 'wrong-default-language',
    version: 'v1.0.0-beta.2',
    module: () => import('./scripts/0001_migration_beta1-beta2').then(m => m.default)
  },
  {
    name: 'new-config',
    version: 'v1.0.0-beta.7',
    module: () => import('./scripts/0002_migration_beta6-beta7').then(m => m.default)
  }
];

const migrationVersionKey = 'zwoo:migrate.version';

export class MigrationRunner {
  static lastVersion = localStorage.getItem(migrationVersionKey);

  static async run(from: string | null, to: string) {
    Logger.info('running migrations...');
    from = from || 'v0.0.0';
    for (const migration of allMigrations) {
      if (semverCompare(to, migration.version) >= 0 && semverCompare(from, migration.version) === -1) {
        try {
          Logger.info(`running ${migration.name} up`);
          await (await migration.module()).up();
        } catch (err: unknown) {
          Logger.warn(`migration failed: ${err}`);
        }
      } else if (semverCompare(to, migration.version) < 0 && semverCompare(from, migration.version) >= 0) {
        try {
          Logger.info(`running ${migration.name} down`);
          await (await migration.module()).down();
        } catch (err: unknown) {
          Logger.warn(`migration failed: ${err}`);
        }
      }
    }
    localStorage.setItem(migrationVersionKey, to);
  }
}
