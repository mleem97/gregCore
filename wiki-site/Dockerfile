# syntax=docker/dockerfile:1
# gregCore Wiki (Astro Starlight, static) — build stage + nginx serve stage.
# Build context is the gregCore repo root (needs .wiki/ + wiki-site/).
# Portainer: repository stack, compose path wiki-site/docker-compose.yml,
# build context = repository root (see docker-compose.yml).
ARG NODE_IMAGE=node:22-alpine
ARG NGINX_IMAGE=nginx:1.29-alpine

FROM ${NODE_IMAGE} AS build
WORKDIR /site
COPY wiki-site/package.json wiki-site/package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY wiki-site/ ./
COPY .wiki/ ../.wiki/
RUN npm run build

FROM ${NGINX_IMAGE} AS serve
COPY --from=build /site/dist /usr/share/nginx/html
COPY wiki-site/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
