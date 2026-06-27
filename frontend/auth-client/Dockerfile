FROM node:22-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

ENV NG_CLI_ANALYTICS=false

COPY . .

EXPOSE 4200

# التعديل هنا: أضفنا --poll 2000 عشان يتغلب على مشاكل المزامنة في الـ Volumes والـ Alpine
CMD ["npx", "ng", "serve", "--host", "0.0.0.0", "--port", "4200", "--poll", "2000"]