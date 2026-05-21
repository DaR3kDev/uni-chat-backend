#!/usr/bin/env sh
# Configura core.hooksPath para usar .githooks del repositorio.
set -eu

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

if ! git rev-parse --git-dir >/dev/null 2>&1; then
  echo "Error: no es un repositorio Git." >&2
  exit 1
fi

git config core.hooksPath .githooks
chmod +x .githooks/pre-commit
chmod +x scripts/render-env.sh 2>/dev/null || true
chmod +x scripts/install-git-hooks.sh

echo "Hooks configurados: core.hooksPath=.githooks"
echo "Pre-commit ejecutará: cd uni-chat-backend && make pre-commit"
