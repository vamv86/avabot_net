# Etapa de build
FROM node:18-alpine AS build
WORKDIR /app

# Copiamos archivos y dependencias
COPY package*.json ./
RUN npm ci

# Copiamos el resto de los archivos
COPY . .

# Construimos el proyecto
RUN npm run build

# Etapa de producción con nginx
FROM nginx:alpine AS production
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
