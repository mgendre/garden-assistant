FROM docker.io/library/node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

FROM docker.io/library/nginx:1.27-alpine AS runtime

RUN rm /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/garden-assistant-app/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
