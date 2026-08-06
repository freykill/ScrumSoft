# ---------------------------------------------------------------
# Compilacion de la SPA
# ---------------------------------------------------------------
FROM node:20-alpine AS build
WORKDIR /app

# npm ci instala exactamente lo del package-lock: dos builds del mismo commit
# dan el mismo resultado, cosa que npm install no garantiza.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration=production

# ---------------------------------------------------------------
# Publicacion
# ---------------------------------------------------------------
FROM nginx:alpine AS final

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/sakai-ng /usr/share/nginx/html

EXPOSE 80
